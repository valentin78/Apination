using System;
using Quartz;
using Sage50Connector.Core;
using Sage50Connector.Models.BindingTypes;

namespace Sage50Connector.Processing
{
    /// <summary>
    /// Observe Apination for data changes and activate appropriate actions for update Sage50 DB
    /// </summary>
    [DisallowConcurrentExecution]
    class ApinationObserver : BaseObserver
    {
        public override void Execute(IJobExecutionContext context)
        {
            try
            {
                // TODO: add logic
                // activate apination action CreateCustomer event, sample
                var action = TypeUtil.ActivateActionByEventBindingType(ApinationEventBindingTypes.CreateCustomer);
                action.Execute(
                    Sage50Api.Value, 
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
