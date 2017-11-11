using Sage50Connector.Models;

namespace Sage50Connector.Processes.Triggers
{
    [EventBinding(Type = EventBindingTypes.CreatedCustomer)]
    class CustomerCreatedTrigger: ISage50Trigger
    {
    }
}
