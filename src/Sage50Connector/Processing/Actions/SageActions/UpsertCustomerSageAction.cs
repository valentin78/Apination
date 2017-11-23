// ReSharper disable InconsistentNaming

using Sage50Connector.Processing.Actions.SageActions.Payloads;

namespace Sage50Connector.Processing.Actions.SageActions
{
    public class UpsertCustomerSageAction : SageAction
    {
        public UpsertCustomerPayload payload { get; set; }
    }
}