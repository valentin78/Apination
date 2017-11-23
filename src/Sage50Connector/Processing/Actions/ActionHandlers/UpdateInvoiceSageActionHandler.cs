using log4net;
using Sage50Connector.API;
using Sage50Connector.Processing.Actions.SageActions;

namespace Sage50Connector.Processing.Actions.ActionHandlers
{
    class UpdateInvoiceSageActionHandler : ISageActionHandler<UpdateInvoiceSageAction>
    {
        public static readonly ILog Log = LogManager.GetLogger(typeof(UpdateInvoiceSageActionHandler));
        private readonly Sage50Api api = new Sage50Api();

        public bool Handle(UpdateInvoiceSageAction action)
        {
            Log.InfoFormat("Open Sage50 company: \"{0}\"", action.payload.companyName);
            api.OpenCompany(action.payload.companyName);

            Log.Info("Update Invoice Data to Sage50 ...");
            api.UpdateInvoice(action.payload.invoice);
            Log.Info("Success!");

            return true;
        }

        public void Dispose()
        {
            api?.Dispose();
        }
    }
}
