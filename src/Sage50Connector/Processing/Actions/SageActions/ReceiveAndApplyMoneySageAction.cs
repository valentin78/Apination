using Sage50Connector.Models.Payloads;

namespace Sage50Connector.Processing.Actions.SageActions
{
    public class ReceiveAndApplyMoneySageAction : SageAction
    {
        public ReceiveAndApplyMoneyPayload payload { get; set; }
    }
}