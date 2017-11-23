// ReSharper disable InconsistentNaming
using Sage50Connector.Models.Data;

namespace Sage50Connector.Models.Payloads
{
    public class UpsertCustomerPayload : Payload
    {
        public Customer customer { get; set; }
    }
}