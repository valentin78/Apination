using Sage50Connector.Models.Data;
// ReSharper disable InconsistentNaming

namespace Sage50Connector.Models.Payloads
{
    public class ReceiveAndApplyMoneyPayload : Payload
    {
        public string invoiceNumber { get; set; }

        public Receipt[] receipts { get; set; }

        public Customer customer { get; set; }
    }
}