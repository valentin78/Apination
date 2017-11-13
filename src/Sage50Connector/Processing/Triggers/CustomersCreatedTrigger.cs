using System.Collections.Generic;
using log4net;
using Sage.Peachtree.API;
using Sage50Connector.API;
using Sage50Connector.Core;
using Sage50Connector.Models;
using Sage50Connector.Models.BindingTypes;

namespace Sage50Connector.Processing.Triggers
{
    [EventBinding(Type = (byte)Sage50EventBindingTypes.CreatedCustomers)]
    class CustomersCreatedTrigger: ISage50Trigger
    {
        public static readonly ILog Log = LogManager.GetLogger(typeof(CustomersCreatedTrigger));

        public void Execute(ApinationApi api, Sage50TriggersConfig triggerConfig, object payload)
        {
            var list = (List<Customer>) payload;
            
            Log.Info($"Received Customers list count: {list.Count}");

            foreach (var customer in list)
            {
                Log.InfoFormat("-> Name: {0}; LastSavedAt: {1}", customer.Name, customer.LastSavedAt);
            }

            //throw new System.NotImplementedException();
        }
    }
}
