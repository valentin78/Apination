using Sage50Connector.API;
using Sage50Connector.Core;
using Sage50Connector.Models;

namespace Sage50Connector.Processes.Triggers
{
    [EventBinding(Type = EventBindingTypes.CreatedCustomer)]
    class CustomerCreatedTrigger: ISage50Trigger
    {
        public void Execute(ApinationApi api, Sage50TriggersConfig triggerConfig)
        {
            throw new System.NotImplementedException();
        }
    }
}
