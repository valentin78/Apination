using Sage50Connector.Models.Payloads;

// ReSharper disable InconsistentNaming

namespace Sage50Connector.Processing.Actions.SageActions
{
    public class CreateInvoiceSageAction : SageAction
    {
        public SalesInvoicePayload payload { get; set; }
    }
}