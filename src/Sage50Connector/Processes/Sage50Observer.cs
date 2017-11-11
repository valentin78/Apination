using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    class Sage50Observer: ProcessBase
    {
        protected override void Process(IJobExecutionContext context)
        {
            var config = context.JobParam<Config>("Config");
            var apinationApi = context.JobParam<ApinationApi>("ApinationApi");

            // TODO: add logic
            var bindingType = EventBindingTypes.CreatedCustomer;
            var action = ProcessesUtil.ActivateByEventBinding<ISage50Trigger>(bindingType);
            
            var triggerConfig = config.TriggersConfig.SingleOrDefault(c => c.TriggerBindingType == bindingType);
            if (triggerConfig == null) throw new ArgumentException($"Config for trigger type {bindingType} not find");
            
            action.Execute(apinationApi, triggerConfig);
        }
    }
}
