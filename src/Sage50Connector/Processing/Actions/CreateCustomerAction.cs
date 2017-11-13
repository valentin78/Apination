using log4net;
using Sage50Connector.API;
using Sage50Connector.Core;
using Sage50Connector.Models.BindingTypes;
using Sage50Connector.Processing.Triggers;

namespace Sage50Connector.Processing.Actions
{
    [EventBinding(Type = (byte)ApinationEventBindingTypes.CreateCustomer)]
    class CreateCustomerAction: IApinationAction<CreateCustomerModel>
    {
        public static readonly ILog Log = LogManager.GetLogger(typeof(CreateCustomerAction));

        public void Execute(Sage50Api api, CreateCustomerModel model)
        {
            Log.Info($"CreateCustomerAction Executed, model: {model}");
        }
    }

    // TODO: fill model by DTO requirements
    class CreateCustomerModel
    {
        public object CustomerData { get; set; }
    }
}
