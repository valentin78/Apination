using Sage50Connector.Models.Payloads;

namespace Sage50Connector.Processing.Actions.SageActions
{
    public class CreatePaymentSageAction : SageAction
    {
        public PaymentPayload payload { get; set; }
    }
}