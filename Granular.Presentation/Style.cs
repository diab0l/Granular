using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Markup;

namespace System.Windows
{
    [ContentProperty("Setters")]
    [DictionaryKeyProperty("Key")]
    public class Style
    {
        private Type targetType;
        public Type TargetType
        {
            get { return targetType; }
            set
            {
                if (targetType == value)
                {
                    return;
                }

                targetType = value;
            }
        }

        private object key;
        public object Key
        {
            get { return key ?? new StyleKey(TargetType); }
            set { key = value; }
        }

        public Style BasedOn { get; set; }
        public List<ITrigger> Triggers { get; private set; }
        public List<ITriggerAction> Setters { get; private set; }
        //public ResourceDictionary Resources { get; set; }

        public Style()
        {
            Triggers = new List<ITrigger>();
            Setters = new List<ITriggerAction>();
        }

        public void Attach(FrameworkElement element)
        {
            Attach(element, setter => true);
        }

        public void Attach(FrameworkElement element, Func<ITriggerAction, bool> settersFilter)
        {
            if (BasedOn != null)
            {
                BasedOn.Attach(element, setter => !Setters.Any(action => action.IsActionOverlaps(setter)) && settersFilter(setter));
            }

            foreach (ITriggerAction action in Setters.Where(settersFilter))
            {
                action.EnterAction(element, BaseValueSource.Style);
            }

            foreach (ITrigger trigger in Triggers)
            {
                trigger.Attach(element, BaseValueSource.StyleTrigger);
            }
        }

        public void Detach(FrameworkElement element)
        {
            Detach(element, setter => true);
        }

        public void Detach(FrameworkElement element, Func<ITriggerAction, bool> settersFilter)
        {
            if (BasedOn != null)
            {
                BasedOn.Detach(element, setter => !Setters.Any(action => action.IsActionOverlaps(setter)) && settersFilter(setter));
            }

            foreach (ITriggerAction action in Setters.Where(settersFilter))
            {
                action.ExitAction(element, BaseValueSource.Style);
            }

            foreach (ITrigger trigger in Triggers)
            {
                trigger.Detach(element, BaseValueSource.StyleTrigger);
            }
        }
    }

    public sealed class StyleKey : IResourceKey
    {
        public Assembly Assembly { get { return TargetType != null ? TargetType.Assembly : null; } }

        public Type TargetType { get; private set; }

        public StyleKey(Type targetType)
        {
            this.TargetType = targetType;
        }

        public override bool Equals(object obj)
        {
            StyleKey other = obj as StyleKey;

            return Object.ReferenceEquals(this, other) || !Object.ReferenceEquals(other, null) &&
                Object.Equals(this.TargetType, other.TargetType);
        }

        public override int GetHashCode()
        {
            return TargetType.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("StyleKey({0})", TargetType.Name);
        }
    }
}
