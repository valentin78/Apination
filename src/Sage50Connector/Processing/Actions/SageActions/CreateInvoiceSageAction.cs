using Sage50Connector.Models.Payloads;
using Sage50Connector.Processing.Actions.SageActions.Payloads;

// ReSharper disable InconsistentNaming

namespace Sage50Connector.Processing.Actions.SageActions
{
    public class CreateInvoiceSageAction : SageAction
    {
        public SalesInvoicePayload payload { get; set; }
    }
}