using Sage50Connector.API;
using Sage50Connector.Core;
using Sage50Connector.Models;

namespace Sage50Connector.Processing.Actions
{
    [EventBinding(Type = EventBindingTypes.CreateCustomer)]
    class CreateCustomerAction: IApinationAction
    {
        public void Execute(Sage50Api api, object payload)
        {
            throw new System.NotImplementedException();
        }
    }
}
