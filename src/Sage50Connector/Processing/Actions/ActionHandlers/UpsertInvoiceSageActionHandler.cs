using log4net;
using Sage50Connector.API;
using Sage50Connector.Processing.Actions.SageActions;

namespace Sage50Connector.Processing.Actions.ActionHandlers
{
    class UpsertInvoiceSageActionHandler : ISageActionHandler<UpsertInvoiceSageAction>
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(UpsertInvoiceSageActionHandler));

        public void Handle(UpsertInvoiceSageAction action)
        {
            using (var api = new Sage50Api(action.source))
            {
                Log.InfoFormat("Open Sage50 company: \"{0}\"", action.payload.companyName);
                api.OpenCompany(action.payload.companyName);

                Log.Info("Upsert Invoice Data to Sage50 ...");
                api.UpsertInvoice(action.payload.invoice);
                Log.Info($"Successfully upserted invoice: {action.payload.invoice.ReferenceNumber}");
            }
        }
    }
}
