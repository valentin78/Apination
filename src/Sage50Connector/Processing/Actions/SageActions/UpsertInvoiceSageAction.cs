// ReSharper disable InconsistentNaming

using Sage50Connector.Models.Payloads;

namespace Sage50Connector.Processing.Actions.SageActions
{
    public class UpsertInvoiceSageAction : SageAction
    {
        public SalesInvoicePayload payload { get; set; }
    }
}