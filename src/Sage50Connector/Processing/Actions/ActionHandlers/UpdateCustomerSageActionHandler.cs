using System;
using log4net;
using Sage50Connector.API;
using Sage50Connector.Processing.Actions.SageActions;

namespace Sage50Connector.Processing.Actions.ActionHandlers
{
    /// <summary>
    /// Update Customer to Sage50 action handler
    /// </summary>
    class UpdateCustomerSageActionHandler: ISageActionHandler
    {
        public static readonly ILog Log = LogManager.GetLogger(typeof(UpdateCustomerSageActionHandler));
        private readonly Sage50Api api = new Sage50Api();

        public bool Handle(SageAction action)
        {
            if (!(action is UpdateCustomerSageAction updateCustomerSageAction)) 
                throw new ArgumentException($"Type for argument {nameof(action)} invalid. Must be <UpdateCustomerSageAction>");

            Log.InfoFormat("Open Sage50 company: \"{0}\"", action.companyName);
            api.OpenCompany(action.companyName);

            Log.Info("Create or Update Customer Data ...");
            api.CreateOrUpdateCustomer(updateCustomerSageAction.payload);
            Log.Info("Success!");

            return true;
        }

        public void Dispose()
        {
            api?.Dispose();
        }
    }
}
