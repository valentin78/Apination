using Sage50Connector.Models.Data;
// ReSharper disable InconsistentNaming

namespace Sage50Connector.Models.Payloads
{
    public class PaymentPayload : Payload
    {
        public SalesInvoice invoice { get; set; }

        public Payment[] payments { get; set; }
    }
}