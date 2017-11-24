namespace Sage50Connector.Models.Data
{
    public class PaymentInvoiceLine : TransactionLine
    {
        public decimal DiscountAmount { get; set; }
        public decimal AmountPaid { get; set; }
    }
}