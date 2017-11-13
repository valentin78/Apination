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
    class CustomersCreatedTrigger: ISage50Trigger<CustomersCreatedModel>
    {
        public static readonly ILog Log = LogManager.GetLogger(typeof(CustomersCreatedTrigger));

        public void Execute(ApinationApi api, Sage50TriggersConfig triggerConfig, CustomersCreatedModel model)
        {
            Log.Info($"Received Customers list count: {model.CustomersList.Count}");

            foreach (var customer in model.CustomersList)
            {
                Log.InfoFormat("-> Name: {0}; LastSavedAt: {1}", customer.Name, customer.LastSavedAt);
            }

            //throw new System.NotImplementedException();
        }
    }

    // TODO: fill model by DTO requirements
    class CustomersCreatedModel
    {
        public List<Customer> CustomersList { get; set; }
    }
}
