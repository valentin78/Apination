using log4net;
using Sage50Connector.API;
using Sage50Connector.Processing.Actions.SageActions;

namespace Sage50Connector.Processing.Actions.ActionHandlers
{
    class UpsertInvoiceSageActionHandler : ISageActionHandler<UpsertInvoiceSageAction>
    {
        public static readonly ILog Log = LogManager.GetLogger(typeof(UpsertInvoiceSageActionHandler));
        
        // ReSharper disable once InconsistentNaming
        private Sage50Api api;

        public void Handle(UpsertInvoiceSageAction action)
        {
            api = new Sage50Api(action.source);
            Log.InfoFormat("Open Sage50 company: \"{0}\"", action.payload.companyName);
            api.OpenCompany(action.payload.companyName);

            Log.Info("Upsert Invoice Data to Sage50 ...");
            api.UpsertInvoice(action.payload.invoice);
            Log.Info("Success!");
        }

        public void Dispose()
        {
            api?.Dispose();
        }
    }
}
