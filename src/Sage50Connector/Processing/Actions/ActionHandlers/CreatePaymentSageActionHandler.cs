using log4net;
using Sage50Connector.API;
using Sage50Connector.Processing.Actions.SageActions;

namespace Sage50Connector.Processing.Actions.ActionHandlers
{
    class CreatePaymentSageActionHandler : ISageActionHandler<CreatePaymentSageAction>
    {
        public static readonly ILog Log = LogManager.GetLogger(typeof(CreatePaymentSageActionHandler));
        private readonly Sage50Api api = new Sage50Api();

        public void Handle(CreatePaymentSageAction action)
        {
            Log.InfoFormat("Open Sage50 company: \"{0}\"", action.payload.companyName);
            api.OpenCompany(action.payload.companyName);

            Log.Info("Create Payment Data to Sage50 ...");
            api.CreatePayment(action.payload);
            Log.Info("Success!");
        }

        public void Dispose()
        {
            api?.Dispose();
        }
    }
}
