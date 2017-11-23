using System;
using log4net;
using Sage50Connector.API;
using Sage50Connector.Processing.Actions.SageActions;

namespace Sage50Connector.Processing.Actions.ActionHandlers
{
    /// <summary>
    /// Update Customer to Sage50 action handler
    /// </summary>
    class UpsertCustomerSageActionHandler: ISageActionHandler<UpsertCustomerSageAction>
    {
        public static readonly ILog Log = LogManager.GetLogger(typeof(UpsertCustomerSageActionHandler));
        private readonly Sage50Api api = new Sage50Api();

        public void Handle(UpsertCustomerSageAction action)
        {
            Log.InfoFormat("Open Sage50 company: \"{0}\"", action.payload.companyName);
            api.OpenCompany(action.payload.companyName);

            Log.Info("Create or Update Customer Data ...");
            api.CreateOrUpdateCustomer(action.payload.customer);
            Log.Info("Success!");
        }

        public void Dispose()
        {
            api?.Dispose();
        }
    }
}
