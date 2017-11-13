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
                // call Apination by URL and get JSON Responce
                var apinationUrl = Config.ApinationActionEndpointUrl;

                // TODO: add processing apination responce logic

                // sample to activate apination action CreateCustomer event
                OnCreateCustomer(customerData: new object());
            }
            catch (Exception ex)
            {
                Log.Error("Job execution failure", ex);
            }
        }

        private void OnCreateCustomer(object customerData)
        {
            var action = TypeUtil.ActivateActionByEventBindingType<CreateCustomerModel>(ApinationEventBindingTypes.CreateCustomer);
                action.Execute(
                    Sage50Api.Value, 
                    new CreateCustomerModel
                    {
                        CustomerData = customerData
                    }
                );
        }
    }
}
