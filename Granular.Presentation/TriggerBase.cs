using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Windows
{
    public interface ITrigger
    {
        void Attach(FrameworkElement element, BaseValueSource valueSource);
        void Detach(FrameworkElement element, BaseValueSource valueSource);
    }

    public interface ITriggerAction
    {
        bool IsActionOverlaps(ITriggerAction action);
        void Apply(FrameworkElement target, BaseValueSource valueSource);
        void Clean(FrameworkElement target, BaseValueSource valueSource);
    }

    public interface IDataTriggerCondition
    {
        event EventHandler IsMatchedChanged;
        bool IsMatched { get; }
    }

    public interface IEventTriggerCondition
    {
        event EventHandler EventRaised;
    }

    public interface IDataTriggerConditionProvider
    {
        IDataTriggerCondition CreateDataTriggerCondition(FrameworkElement element);
    }

    public abstract class TriggerBase : Freezable, ITrigger
    {
        protected abstract IEnumerable<ITriggerAction> TriggerActions { get; }

        public abstract void Attach(FrameworkElement element, BaseValueSource valueSource);

        public abstract void Detach(FrameworkElement element, BaseValueSource valueSource);
    }

    public abstract class DataTriggerBase : TriggerBase, IDataTriggerConditionProvider
    {
        private Dictionary<FrameworkElement, IDataTriggerCondition> attachedConditions;

        public DataTriggerBase()
        {
            attachedConditions = new Dictionary<FrameworkElement, IDataTriggerCondition>();
        }

        public abstract IDataTriggerCondition CreateDataTriggerCondition(FrameworkElement element);

        public override void Attach(FrameworkElement element, BaseValueSource valueSource)
        {
            IDataTriggerCondition condition = CreateDataTriggerCondition(element);

            condition.IsMatchedChanged += (sender, e) => OnConditionIsMatchedChanged(element, valueSource, condition.IsMatched);

            if (condition.IsMatched)
            {
                Apply(element, valueSource);
            }

            attachedConditions.Add(element, condition);
        }

        public override void Detach(FrameworkElement element, BaseValueSource valueSource)
        {
            IDataTriggerCondition condition = attachedConditions[element];

            if (condition.IsMatched)
            {
                Clean(element, valueSource);
            }

            if (condition is IDisposable)
            {
                ((IDisposable)condition).Dispose();
            }

            attachedConditions.Remove(element);
        }

        private void OnConditionIsMatchedChanged(FrameworkElement element, BaseValueSource valueSource, bool isMatched)
        {
            if (isMatched)
            {
                Apply(element, valueSource);
            }
            else
            {
                Clean(element, valueSource);
            }
        }

        private void Apply(FrameworkElement element, BaseValueSource valueSource)
        {
            foreach (ITriggerAction action in TriggerActions)
            {
                action.Apply(element, valueSource);
            }
        }

        private void Clean(FrameworkElement element, BaseValueSource valueSource)
        {
            foreach (ITriggerAction action in TriggerActions)
            {
                action.Clean(element, valueSource);
            }
        }
    }

    public abstract class EventTriggerBase : TriggerBase
    {
        private Dictionary<FrameworkElement, IEventTriggerCondition> attachedConditions;

        public EventTriggerBase()
        {
            attachedConditions = new Dictionary<FrameworkElement, IEventTriggerCondition>();
        }

        public abstract IEventTriggerCondition CreateEventTriggerCondition(FrameworkElement element);

        public override void Attach(FrameworkElement element, BaseValueSource valueSource)
        {
            IEventTriggerCondition condition = CreateEventTriggerCondition(element);

            condition.EventRaised += (sender, e) => Apply(element, valueSource);

            attachedConditions.Add(element, condition);
        }

        public override void Detach(FrameworkElement element, BaseValueSource valueSource)
        {
            IEventTriggerCondition condition = attachedConditions[element];

            Clean(element, valueSource);

            if (condition is IDisposable)
            {
                ((IDisposable)condition).Dispose();
            }

            attachedConditions.Remove(element);
        }

        private void Apply(FrameworkElement element, BaseValueSource valueSource)
        {
            foreach (ITriggerAction action in TriggerActions)
            {
                action.Apply(element, valueSource);
            }
        }

        private void Clean(FrameworkElement element, BaseValueSource valueSource)
        {
            foreach (ITriggerAction action in TriggerActions)
            {
                action.Clean(element, valueSource);
            }
        }
    }
}
