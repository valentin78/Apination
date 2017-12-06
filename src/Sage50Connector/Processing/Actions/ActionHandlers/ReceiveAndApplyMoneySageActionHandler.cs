using log4net;
using Sage50Connector.API;
using Sage50Connector.Processing.Actions.SageActions;

namespace Sage50Connector.Processing.Actions.ActionHandlers
{
    class ReceiveAndApplyMoneySageActionHandler : ISageActionHandler<ReceiveAndApplyMoneySageAction>
    {
        public static readonly ILog Log = LogManager.GetLogger(typeof(ReceiveAndApplyMoneySageActionHandler));
        
        // ReSharper disable once InconsistentNaming
        private Sage50Api api;

        public void Handle(ReceiveAndApplyMoneySageAction action)
        {
            api = new Sage50Api(action.source);
            Log.InfoFormat("Open Sage50 company: \"{0}\"", action.payload.companyName);
            api.OpenCompany(action.payload.companyName);

            Log.Info("Create Receipt Data in Sage50 ...");
            api.CreateReceipt(action.payload);
            Log.Info("Success!");
        }

        public void Dispose()
        {
            api?.Dispose();
        }
    }
}
