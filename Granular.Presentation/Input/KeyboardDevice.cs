using System;
using System.Collections.Generic;
using System.Linq;
using Granular;
using Granular.Extensions;

namespace System.Windows.Input
{
    public class RawKeyboardEventArgs : RawEventArgs
    {
        public Key Key { get; private set; }
        public KeyStates KeyStates { get; private set; }
        public bool IsRepeat { get; private set; }

        public RawKeyboardEventArgs(Key key, KeyStates keyStates, bool isRepeat, int timestamp) :
            base(timestamp)
        {
            this.Key = key;
            this.KeyStates = keyStates;
            this.IsRepeat = isRepeat;
        }
    }

    public enum ModifierKeys
    {
        None = 0,
        Alt = 1,
        Control = 2,
        Shift = 4,
        Windows = 8
    }

    public class KeyboardDevice : IInputDevice
    {
        public event KeyEventHandler PreProcessKey;
        public event KeyEventHandler PostProcessKey;

        public event EventHandler TargetChanged;
        private IInputElement target;
        public IInputElement Target
        {
            get { return target; }
            private set
            {
                if (target == value)
                {
                    return;
                }

                IInputElement oldTarget = target;
                target = value;

                OnTargetChanged(oldTarget, target);
                TargetChanged.Raise(this);
            }
        }

        public ModifierKeys Modifiers
        {
            get
            {
                return ((downKeys.Contains(Key.LeftAlt) || downKeys.Contains(Key.RightAlt)) ? ModifierKeys.Alt : ModifierKeys.None) |
                    ((downKeys.Contains(Key.LeftCtrl) || downKeys.Contains(Key.RightCtrl)) ? ModifierKeys.Control : ModifierKeys.None) |
                    ((downKeys.Contains(Key.LeftShift) || downKeys.Contains(Key.RightShift)) ? ModifierKeys.Shift : ModifierKeys.None) |
                    ((downKeys.Contains(Key.LWin) || downKeys.Contains(Key.RWin)) ? ModifierKeys.Windows : ModifierKeys.None);
            }
        }

        private HashSet<Key> downKeys;

        private IPresentationSource presentationSource;

        public KeyboardDevice(IPresentationSource presentationSource)
        {
            this.presentationSource = presentationSource;

            downKeys = new HashSet<Key>();
        }

        public void Activate()
        {
            //
        }

        public void Deactivate()
        {
            if (Target != null)
            {
                int timestamp = presentationSource.GetTimestamp();

                foreach (Key key in downKeys)
                {
                    Target.RaiseEvents(
                        new KeyEventArgs(Keyboard.PreviewKeyUpEvent, Target, this, timestamp, key, KeyStates.None, false),
                        new KeyEventArgs(Keyboard.KeyUpEvent, Target, this, timestamp, key, KeyStates.None, false));
                }
            }

            downKeys.Clear();
        }

        public bool ProcessRawEvent(RawKeyboardEventArgs rawEvent)
        {
            if (rawEvent.KeyStates == KeyStates.Down)
            {
                downKeys.Add(rawEvent.Key);
            }
            else
            {
                downKeys.Remove(rawEvent.Key);
            }

            RoutedEvent previewRoutedEvent;
            RoutedEvent routedEvent;

            if (rawEvent.KeyStates == KeyStates.Down)
            {
                previewRoutedEvent = Keyboard.PreviewKeyDownEvent;
                routedEvent = Keyboard.KeyDownEvent;
            }
            else if (rawEvent.KeyStates == KeyStates.None)
            {
                previewRoutedEvent = Keyboard.PreviewKeyUpEvent;
                routedEvent = Keyboard.KeyUpEvent;
            }
            else
            {
                throw new Granular.Exception("Unexpected KeyStates \"{0}\"", rawEvent.KeyStates);
            }

            KeyEventArgs previewEventArgs = new KeyEventArgs(previewRoutedEvent, Target, this, rawEvent.Timestamp, rawEvent.Key, rawEvent.KeyStates, rawEvent.IsRepeat);
            KeyEventArgs eventArgs = new KeyEventArgs(routedEvent, Target, this, rawEvent.Timestamp, rawEvent.Key, rawEvent.KeyStates, rawEvent.IsRepeat);

            PreProcessKey.Raise(this, previewEventArgs);

            if (Target != null)
            {
                Target.RaiseEvents(previewEventArgs, eventArgs);
            }

            PostProcessKey.Raise(this, eventArgs);

            return (previewEventArgs.Handled || eventArgs.Handled) && !previewEventArgs.ForceHostHandling && !eventArgs.ForceHostHandling;
        }

        public KeyStates GetKeyStates(Key key)
        {
            return downKeys.Contains(key) ? KeyStates.Down : KeyStates.None;
        }

        public IDisposable Focus(IInputElement element)
        {
            Target = element;

            return new Disposable(() =>
            {
                if (Target == element)
                {
                    Target = null;
                }
            });
        }

        private void OnTargetChanged(IInputElement oldTarget, IInputElement newTarget)
        {
            int timestamp = presentationSource.GetTimestamp();

            if (oldTarget != null)
            {
                foreach (Key key in downKeys)
                {
                    oldTarget.RaiseEvents(
                        new KeyEventArgs(Keyboard.PreviewKeyUpEvent, oldTarget, this, timestamp, key, KeyStates.None, false),
                        new KeyEventArgs(Keyboard.KeyUpEvent, oldTarget, this, timestamp, key, KeyStates.None, false));
                }

                oldTarget.RaiseEvents(
                    new KeyboardFocusChangedEventArgs(Keyboard.PreviewLostKeyboardFocusEvent, oldTarget, this, timestamp, oldTarget, newTarget),
                    new KeyboardFocusChangedEventArgs(Keyboard.LostKeyboardFocusEvent, oldTarget, this, timestamp, oldTarget, newTarget));
            }

            if (newTarget != null)
            {
                newTarget.RaiseEvents(
                    new KeyboardFocusChangedEventArgs(Keyboard.PreviewGotKeyboardFocusEvent, newTarget, this, timestamp, oldTarget, newTarget),
                    new KeyboardFocusChangedEventArgs(Keyboard.GotKeyboardFocusEvent, newTarget, this, timestamp, oldTarget, newTarget));

                foreach (Key key in downKeys)
                {
                    newTarget.RaiseEvents(
                        new KeyEventArgs(Keyboard.PreviewKeyDownEvent, newTarget, this, timestamp, key, KeyStates.None, false),
                        new KeyEventArgs(Keyboard.KeyDownEvent, newTarget, this, timestamp, key, KeyStates.None, false));
                }
            }
        }
    }
}
