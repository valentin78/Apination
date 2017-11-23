using log4net;
using Sage50Connector.API;
using Sage50Connector.Processing.Actions.SageActions;

namespace Sage50Connector.Processing.Actions.ActionHandlers
{
    class CreateInvoiceSageActionHandler : ISageActionHandler<CreateInvoiceSageAction>
    {
        public static readonly ILog Log = LogManager.GetLogger(typeof(CreateInvoiceSageActionHandler));
        private readonly Sage50Api api = new Sage50Api();

        public void Handle(CreateInvoiceSageAction action)
        {
            Log.InfoFormat("Open Sage50 company: \"{0}\"", action.payload.companyName);
            api.OpenCompany(action.payload.companyName);

            Log.Info("Create Invoice Data to Sage50 ...");
            api.CreateInvoice(action.payload.invoice);
            Log.Info("Success!");
        }

        public void Dispose()
        {
            api?.Dispose();
        }
    }
}
