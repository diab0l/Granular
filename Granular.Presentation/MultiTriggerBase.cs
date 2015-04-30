using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Granular.Extensions;
using System.Windows.Markup;

namespace System.Windows
{
    internal class MultiDataTriggerCondition : IDataTriggerCondition, IDisposable
    {
        public event EventHandler IsMatchedChanged;
        private bool isMatched;
        public bool IsMatched
        {
            get { return isMatched; }
            private set
            {
                if (isMatched == value)
                {
                    return;
                }

                isMatched = value;
                IsMatchedChanged.Raise(this);
            }
        }

        private FrameworkElement element;
        private IEnumerable<IDataTriggerConditionProvider> conditionProviders;
        private IEnumerable<IDataTriggerCondition> conditions;

        private MultiDataTriggerCondition(FrameworkElement element, IEnumerable<IDataTriggerConditionProvider> conditionProviders)
        {
            this.element = element;
            this.conditionProviders = conditionProviders;
        }

        private void Register()
        {
            conditions = conditionProviders.Select(conditionProvider => conditionProvider.CreateDataTriggerCondition(element)).ToArray();

            foreach (IDataTriggerCondition condition in conditions)
            {
                condition.IsMatchedChanged += OnConditionIsMatchedChanged;
            }

            IsMatched = conditions.All(condition => condition.IsMatched);
        }

        public void Dispose()
        {
            foreach (IDataTriggerCondition condition in conditions)
            {
                condition.IsMatchedChanged -= OnConditionIsMatchedChanged;

                if (condition is IDisposable)
                {
                    ((IDisposable)condition).Dispose();
                }
            }
        }

        private void OnConditionIsMatchedChanged(object sender, EventArgs e)
        {
            IsMatched = conditions.All(condition => condition.IsMatched);
        }

        public static MultiDataTriggerCondition Register(FrameworkElement element, IEnumerable<IDataTriggerConditionProvider> conditionProviders)
        {
            MultiDataTriggerCondition condition = new MultiDataTriggerCondition(element, conditionProviders);
            condition.Register();
            return condition;
        }
    }

    [ContentProperty("Setters")]
    public abstract class MultiDataTriggerBase : DataTriggerBase
    {
        public List<Condition> Conditions { get; private set; }
        public List<ITriggerAction> Setters { get; private set; }

        protected override IEnumerable<ITriggerAction> TriggerActions { get { return Setters; } }

        public MultiDataTriggerBase()
        {
            Conditions = new List<Condition>();
            Setters = new List<ITriggerAction>();
        }
    }
}
