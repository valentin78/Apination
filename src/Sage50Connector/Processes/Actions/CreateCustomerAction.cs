using Sage50Connector.API;
using Sage50Connector.Models;

namespace Sage50Connector.Processes.Actions
{
    [EventBinding(Type = EventBindingTypes.CreatedCustomer)]
    class CreateCustomerAction: IApinationAction
    {
        public void Execute(Sage50Api api)
        {
            throw new System.NotImplementedException();
        }
    }
}
