﻿using log4net;
using Sage50Connector.API;
using Sage50Connector.Processing.Actions.SageActions;

namespace Sage50Connector.Processing.Actions.ActionHandlers
{
    /// <summary>
    /// Update Customer to Sage50 action handler
    /// </summary>
    class UpsertCustomerSageActionHandler: ISageActionHandler<UpsertCustomerSageAction>
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(UpsertCustomerSageActionHandler));
        
        public void Handle(UpsertCustomerSageAction action)
        {
            using (var api = new Sage50Api(action.source))
            {
                Log.InfoFormat("Open Sage50 company: \"{0}\"", action.payload.companyName);
                api.OpenCompany(action.payload.companyName);

                Log.Info("Create or Update Customer Data ...");
                api.CreateOrUpdateCustomer(action.payload.customer);
                Log.Info($"Successfully upserted customer: {action.payload.customer.ExternalId}");
            }
        }
    }
}
