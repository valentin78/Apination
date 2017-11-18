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

        public void Handle(SageAction action)
        {
            Log.InfoFormat("Handling action type: {0}", action.type);
            
            Sage50Api api = new Sage50Api();

            //throw new NotImplementedException();
        }
    }
}
