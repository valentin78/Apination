﻿using log4net;
using Sage50Connector.API;
using Sage50Connector.Processing.Actions.SageActions;

namespace Sage50Connector.Processing.Actions.ActionHandlers
{
    class UpdateInvoiceSageActionHandler : ISageActionHandler<UpdateInvoiceSageAction>
    {
        public static readonly ILog Log = LogManager.GetLogger(typeof(UpdateInvoiceSageActionHandler));
        
        // ReSharper disable once InconsistentNaming
        private Sage50Api api;

        public void Handle(UpdateInvoiceSageAction action)
        {
            api = new Sage50Api(action.source);
            Log.InfoFormat("Open Sage50 company: \"{0}\"", action.payload.companyName);
            api.OpenCompany(action.payload.companyName);

            Log.Info("Update Invoice Data to Sage50 ...");
            api.UpdateInvoice(action.payload.invoice);
            Log.Info("Success!");
        }

        public void Dispose()
        {
            api?.Dispose();
        }
    }
}
