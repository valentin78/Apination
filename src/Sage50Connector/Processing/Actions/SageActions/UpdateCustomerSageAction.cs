using Sage50Connector.Models.Payloads;
// ReSharper disable InconsistentNaming

namespace Sage50Connector.Processing.Actions.SageActions
{
    public class UpdateCustomerSageAction : SageAction
    {
        public Customer payload { get; set; }
    }
}