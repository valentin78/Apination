using Sage50Connector.Models.Payloads;

namespace Sage50Connector.Processing.Actions.SageActions.Payloads
{
    public class UpsertCustomerPayload : Payload
    {
        public Customer customer { get; set; }
    }
}