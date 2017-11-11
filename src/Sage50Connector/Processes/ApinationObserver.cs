using System;
using Quartz;
using Sage50Connector.API;
using Sage50Connector.Core;
using Sage50Connector.Models;
using Sage50Connector.Processes.Actions;

namespace Sage50Connector.Processes
{
    /// <summary>
    /// Observe Apination for data changes and activate appropriate actions for update Sage50 DB
    /// </summary>
    [DisallowConcurrentExecution]
    class ApinationObserver : ProcessBase
    {
        protected override void Process(IJobExecutionContext context)
        {
            var config = context.JobParam<Config>("Config");
            var sage50Api = context.JobParam<Sage50Api>("Sage50Api");

            var apinationDTOUrl = config.ApinationDTOToSage50Url;

            // TODO: add logic
            var action = ProcessesUtil.ActivateByEventBinding<IApinationAction>(EventBindingTypes.CreatedCustomer);
            action.Execute(sage50Api);
        }
    }
}
