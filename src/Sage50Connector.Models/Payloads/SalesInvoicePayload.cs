using Sage50Connector.Models.Data;
// ReSharper disable InconsistentNaming

namespace Sage50Connector.Models.Payloads
{
    public class SalesInvoicePayload : Payload
    {
        public SalesInvoice invoice { get; set; }
    }
}