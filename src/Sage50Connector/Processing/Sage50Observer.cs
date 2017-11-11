using System;
using System.Linq;
using log4net;
using Quartz;
using Sage50Connector.API;
using Sage50Connector.Core;
using Sage50Connector.Models;
using Sage50Connector.Processing.Triggers;

namespace Sage50Connector.Processing
{
    /// <summary>
    /// Observe Sage50 for data changes and activate appropriate triggers for DTO to Apination
    /// </summary>
    [DisallowConcurrentExecution]
    class Sage50Observer : IJob
    {
        /// <summary>
        /// ILog instance for logging purpose
        /// </summary>
        public static readonly ILog Log = LogManager.GetLogger(typeof(Sage50Observer));

        /// <summary>
        /// Apination Api Util
        /// </summary>
        protected ApinationApi ApinationApi => new ApinationApi(new WebClientHttpUtility());

        public Config Config { private get; set; }

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                // TODO: add logic

                // activate sage50 trigger for CreateCustomer event, sample
                var bindingType = EventBindingTypes.CreatedCustomers;
                var action = TypeUtil.ActivateByEventBinding<ISage50Trigger>(bindingType);

                var triggerConfig = Config.TriggersConfig.SingleOrDefault(c => c.TriggerBindingType == bindingType);
                if (triggerConfig == null) throw new ArgumentException($"Config for trigger type {bindingType} not find");

                action.Execute(
                    ApinationApi, triggerConfig, 
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
