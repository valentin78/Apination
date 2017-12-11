using log4net;
using Sage50Connector.API;
using Sage50Connector.Processing.Actions.SageActions;

namespace Sage50Connector.Processing.Actions.ActionHandlers
{
    class ReceiveAndApplyMoneySageActionHandler : ISageActionHandler<ReceiveAndApplyMoneySageAction>
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ReceiveAndApplyMoneySageActionHandler));
        
        public void Handle(ReceiveAndApplyMoneySageAction action)
        {
            using (var api = new Sage50Api(action.source))
            {
                Log.InfoFormat("Open Sage50 company: \"{0}\"", action.payload.companyName);
                api.OpenCompany(action.payload.companyName);

                Log.Info("Create Receipts in Sage50 ...");
                api.CreateReceipt(action.payload);
                Log.Info($"Successfully created Receipts for invoice: {action.payload.invoiceNumber}");
            }
        }
    }
}
