using System;
using log4net;
using Sage50Connector.Processing.Actions.ActionHandlers.Factory;
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
            //throw new NotImplementedException();
        }
    }
}
