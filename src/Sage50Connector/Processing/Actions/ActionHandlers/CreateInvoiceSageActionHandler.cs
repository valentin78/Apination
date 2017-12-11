using log4net;
using Sage50Connector.API;
using Sage50Connector.Processing.Actions.SageActions;

namespace Sage50Connector.Processing.Actions.ActionHandlers
{
    class CreateInvoiceSageActionHandler : ISageActionHandler<CreateInvoiceSageAction>
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(CreateInvoiceSageActionHandler));

        public void Handle(CreateInvoiceSageAction action)
        {
            using (var api = new Sage50Api(action.source))
            {
                Log.InfoFormat("Open Sage50 company: \"{0}\"", action.payload.companyName);
                api.OpenCompany(action.payload.companyName);

                Log.Info("Create Invoice Data to Sage50 ...");
                api.CreateInvoice(action.payload.invoice);
                Log.Info($"Successfully created invoice: {action.payload.invoice.ReferenceNumber}");
            }
        }
    }
}
