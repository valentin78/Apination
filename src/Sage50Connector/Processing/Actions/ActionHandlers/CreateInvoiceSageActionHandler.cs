using System;
using log4net;
using Sage50Connector.API;
using Sage50Connector.Processing.Actions.SageActions;

namespace Sage50Connector.Processing.Actions.ActionHandlers
{
    /// <summary>
    /// Update Customer to Sage50 action handler
    /// </summary>
    class CreateInvoiceSageActionHandler : ISageActionHandler
    {
        public static readonly ILog Log = LogManager.GetLogger(typeof(CreateInvoiceSageActionHandler));
        private readonly Sage50Api api = new Sage50Api();

        public void Handle(SageAction action)
        {
            Log.InfoFormat("Handling action type: {0}", action.type);

            if (!(action is CreateInvoiceSageAction createInvoiceSageAction)) 
                throw new ArgumentException($"Type for argument {nameof(action)} invalid. Must be <CreateInvoiceSageAction>");

            Log.InfoFormat("Open Sage50 company: \"{0}\"", action.companyName);
            api.OpenCompany(action.companyName);

            Log.Info("Create Invoice Data ...");

            // TODO: implement this
            //api.CreateOrUpdateCustomer(updateCustomerSageAction.payload);
            //Log.Info("Success!");
        }

        public void Dispose()
        {
            api?.Dispose();
        }
    }
}
