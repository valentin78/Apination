using Sage50Connector.Models.Payloads;

namespace Sage50Connector.Processing.Actions.SageActions.Payloads
{
    public class SalesInvoicePayload : Payload
    {
        public SalesInvoice invoice { get; set; }
    }
}