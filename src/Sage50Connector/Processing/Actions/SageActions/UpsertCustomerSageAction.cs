// ReSharper disable InconsistentNaming

using Sage50Connector.Models.Payloads;

namespace Sage50Connector.Processing.Actions.SageActions
{
    public class UpsertCustomerSageAction : SageAction
    {
        public UpsertCustomerPayload payload { get; set; }
    }
}