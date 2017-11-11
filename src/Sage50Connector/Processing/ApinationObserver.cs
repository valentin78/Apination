using System;
using log4net;
using Quartz;
using Sage50Connector.API;
using Sage50Connector.Core;
using Sage50Connector.Models;
using Sage50Connector.Processing.Actions;

namespace Sage50Connector.Processing
{
    /// <summary>
    /// Observe Apination for data changes and activate appropriate actions for update Sage50 DB
    /// </summary>
    [DisallowConcurrentExecution]
    class ApinationObserver : IJob
    {
        /// <summary>
        /// ILog instance for logging purpose
        /// </summary>
        public static readonly ILog Log = LogManager.GetLogger(typeof(ApinationObserver));

        /// <summary>
        /// Sage50 Api
        /// </summary>
        protected Sage50Api Sage50Api => new Sage50Api();

        public Config Config { private get; set; }

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                var apinationDTOUrl = Config.ApinationDTOToSage50Url;

                // TODO: add logic
                // activate apination action CreateCustomer event, sample
                var action = TypeUtil.ActivateByEventBinding<IApinationAction>(EventBindingTypes.CreatedCustomer);
                action.Execute(
                    Sage50Api, 
                    payload: new {}
                );
            }
            catch (Exception ex)
            {
                Log.Error("Job execution failure", ex);
            }
        }
    }
}
