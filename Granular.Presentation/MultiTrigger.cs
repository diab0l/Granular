using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using Granular.Extensions;

namespace System.Windows
{
    [ContentProperty("Setters")]
    public class MultiTrigger : MultiDataTriggerBase
    {
        private class TriggerConditionProvider : IDataTriggerConditionProvider
        {
            private Condition condition;

            public TriggerConditionProvider(Condition condition)
            {
                this.condition = condition;
            }

            public IDataTriggerCondition CreateDataTriggerCondition(FrameworkElement element)
            {
                return condition.CreateTriggerCondition(element);
            }
        }

        public override IDataTriggerCondition CreateDataTriggerCondition(FrameworkElement element)
        {
            return MultiDataTriggerCondition.Register(element, Conditions.Select(condition => new TriggerConditionProvider(condition)).ToArray());
        }
    }
}
