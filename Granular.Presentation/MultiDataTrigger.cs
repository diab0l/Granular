using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using Granular.Extensions;
using System.Xaml;

namespace System.Windows
{
    public class MultiDataTrigger : MultiDataTriggerBase
    {
        private class MultiDataTriggerConditionProvider : IDataTriggerConditionProvider
        {
            private Condition condition;

            public MultiDataTriggerConditionProvider(Condition condition)
            {
                this.condition = condition;
            }

            public IDataTriggerCondition CreateDataTriggerCondition(FrameworkElement element)
            {
                return condition.CreateDataTriggerCondition(element);
            }
        }

        public override IDataTriggerCondition CreateDataTriggerCondition(FrameworkElement element)
        {
            return MultiDataTriggerCondition.Register(element, Conditions.Select(condition => new MultiDataTriggerConditionProvider(condition)).ToArray());
        }
    }
}
