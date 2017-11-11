using System;
using System.Diagnostics;
using System.Threading;
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
        public Config Config { private get; set; }

        protected override void Process(IJobExecutionContext context)
        {
            Log.InfoFormat("Apination Config: {0}", Config);
            var apinationDTOUrl = Config.ApinationDTOToSage50Url;

            // TODO: add logic
            var action = ProcessesUtil.ActivateByEventBinding<IApinationAction>(EventBindingTypes.CreatedCustomer);
            action.Execute(Sage50Api);
        }
    }
}
