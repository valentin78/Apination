using Sage50Connector.API;
using Sage50Connector.Core;
using Sage50Connector.Models.BindingTypes;

namespace Sage50Connector.Processing.Actions
{
    [EventBinding(Type = (byte)ApinationEventBindingTypes.CreateCustomer)]
    class CreateCustomerAction: IApinationAction
    {
        public void Execute(Sage50Api api, object payload)
        {
            throw new System.NotImplementedException();
        }
    }
}
