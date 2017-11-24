namespace Sage50Connector.Models.Data
{
        public sealed class PaymentExpenseLine : TransactionLine
        {
            public decimal Quantity { get; set; }
            public decimal UnitPrice { get; set; }
        }
}