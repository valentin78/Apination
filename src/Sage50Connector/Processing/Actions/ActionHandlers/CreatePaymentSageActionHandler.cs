﻿using log4net;
using Sage50Connector.API;
using Sage50Connector.Processing.Actions.SageActions;

namespace Sage50Connector.Processing.Actions.ActionHandlers
{
    class CreatePaymentSageActionHandler : ISageActionHandler<CreatePaymentSageAction>
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(CreatePaymentSageActionHandler));

        public void Handle(CreatePaymentSageAction action)
        {
            using (var api = new Sage50Api(action.source))
            {
                Log.InfoFormat("Open Sage50 company: \"{0}\"", action.payload.companyName);
                api.OpenCompany(action.payload.companyName);

                Log.Info("Create Payment Data to Sage50 ...");
                api.CreatePayment(action.payload);
                Log.Info($"Successfully created payment for invoice: {action.payload.invoice.ReferenceNumber}");
            }
        }
    }
}
