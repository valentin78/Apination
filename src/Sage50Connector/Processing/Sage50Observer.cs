using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Quartz;
using Sage.Peachtree.API;
using Sage50Connector.Core;
using Sage50Connector.Models;
using Sage50Connector.Models.BindingTypes;

namespace Sage50Connector.Processing
{
    /// <summary>
    /// Observe Sage50 for data changes and activate appropriate triggers for DTO to Apination
    /// </summary>
    [DisallowConcurrentExecution]
    class Sage50Observer : BaseObserver
    {
        public override void Execute(IJobExecutionContext context)
        {
            try
            {
                var api = Sage50Api.Value;

                foreach (var company in Config.CompaniesList)
                {
                    Log.Info($"Starting query changes for company: '{company.CompanyName}'");
                    
                    api.OpenCompany(company.CompanyName);

                    var customers = api.CustomersList();
                    
                    // TODO: how add filter before load ?
                    customers.Load();

                    var lastSavedAtBefore = ApplicationConfig.CustomersLastSavedAt;
                    
                    Log.Info($"LastSavedBefore filter: {lastSavedAtBefore }");

                    var customersList = customers.Where(c => c.LastSavedAt > lastSavedAtBefore).ToList();
                    if (customersList.Count > 0)
                    {
                        OnCustomersCreated(customersList);
                        ApplicationConfig.CustomersLastSavedAt = customersList.Max(c => c.LastSavedAt);
                    }
                    else
                    {
                        Log.Info("Customers list is unchanged!");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Job execution failure", ex);
            }
        }

        void OnCustomersCreated(List<Customer> customersList)
        {
            // activate sage50 trigger for CreateCustomer event, sample
            var bindingType = Sage50EventBindingTypes.CreatedCustomers;
            var trigger = TypeUtil.ActivateTriggerByEventBindingType(bindingType);

            var triggerConfig = GetTriggerConfigByBindingType(bindingType);

            trigger.Execute(
                ApinationApi.Value,
                triggerConfig,
                customersList
            );
        }

        private Sage50TriggersConfig GetTriggerConfigByBindingType(Sage50EventBindingTypes bindingType)
        {
            var triggerConfig = Config.TriggersConfig.SingleOrDefault(c => c.TriggerBindingType == bindingType);
            if (triggerConfig == null) throw new ArgumentException($"Config for trigger type {bindingType} not found");
            return triggerConfig;
        }
    }
}
