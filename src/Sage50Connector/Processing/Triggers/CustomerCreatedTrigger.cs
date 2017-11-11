using Sage50Connector.API;
using Sage50Connector.Core;
using Sage50Connector.Models;
using Sage50Connector.Models.BindingTypes;

namespace Sage50Connector.Processing.Triggers
{
    [EventBinding(Type = (byte)Sage50EventBindingTypes.CreatedCustomers)]
    class CustomerCreatedTrigger: ISage50Trigger
    {
        public void Execute(ApinationApi api, Sage50TriggersConfig triggerConfig, object payload)
        {
            throw new System.NotImplementedException();
        }
    }
}
