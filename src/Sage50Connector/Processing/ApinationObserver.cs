using System;
using Quartz;
using Sage50Connector.Core;
using Sage50Connector.Models.BindingTypes;
using Sage50Connector.Processing.Actions;

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
                var apinationDTOUrl = Config.ApinationDTOToSage50Url;

                // TODO: add logic
                // activate apination action CreateCustomer event, sample
                var action = TypeUtil.ActivateActionByEventBindingType<CreateCustomerModel>(ApinationEventBindingTypes.CreateCustomer);
                action.Execute(
                    Sage50Api.Value, 
                    new CreateCustomerModel
                    {
                        // stub
                        CustomerData = new object()
                    }
                );
            }
            catch (Exception ex)
            {
                Log.Error("Job execution failure", ex);
            }
        }
    }
}
