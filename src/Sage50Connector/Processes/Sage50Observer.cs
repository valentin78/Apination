using System;
using System.Linq;
using Quartz;
using Sage50Connector.API;
using Sage50Connector.Core;
using Sage50Connector.Models;
using Sage50Connector.Processes.Triggers;

namespace Sage50Connector.Processes
{
    /// <summary>
    /// Observe Sage50 for data changes and activate appropriate triggers for DTO to Apination
    /// </summary>
    [DisallowConcurrentExecution]
    class Sage50Observer : ProcessBase
    {
        public Config Config { private get; set; }

        protected override void Process(IJobExecutionContext context)
        {
            // TODO: add logic

            // activate sage50 trigger for CreateCustomer event, sample
            var bindingType = EventBindingTypes.CreatedCustomer;
            var action = ProcessesUtil.ActivateByEventBinding<ISage50Trigger>(bindingType);
            
            var triggerConfig = Config.TriggersConfig.SingleOrDefault(c => c.TriggerBindingType == bindingType);
            if (triggerConfig == null) throw new ArgumentException($"Config for trigger type {bindingType} not find");
            
            action.Execute(ApinationApi, triggerConfig);
        }
    }
}
