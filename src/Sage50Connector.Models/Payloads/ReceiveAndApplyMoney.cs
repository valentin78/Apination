using Sage50Connector.Models.Data;
// ReSharper disable InconsistentNaming

namespace Sage50Connector.Models.Payloads
{
    public class ReceiveAndApplyMoneyPayload : Payload
    {
        public SalesInvoice invoice { get; set; }

        public Receipt[] receipts { get; set; }
    }
}