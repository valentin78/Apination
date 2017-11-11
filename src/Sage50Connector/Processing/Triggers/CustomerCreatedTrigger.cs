using Sage50Connector.API;
using Sage50Connector.Core;
using Sage50Connector.Models;

namespace Sage50Connector.Processing.Triggers
{
    [EventBinding(Type = EventBindingTypes.CreatedCustomers)]
    class CustomerCreatedTrigger: ISage50Trigger
    {
        public void Execute(ApinationApi api, Sage50TriggersConfig triggerConfig, object payload)
        {
            throw new System.NotImplementedException();
        }
    }
}
