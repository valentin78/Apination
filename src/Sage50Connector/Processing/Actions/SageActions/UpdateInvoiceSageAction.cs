using Sage50Connector.Models.Payloads;
// ReSharper disable InconsistentNaming

namespace Sage50Connector.Processing.Actions.SageActions
{
    public class UpdateInvoiceSageAction : SageAction
    {
        public SalesInvoice payload { get; set; }
    }
}