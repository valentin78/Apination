namespace Sage50Connector.Models.Data
{
    public class ReceiptInvoiceLine : TransactionLine
    {
        public decimal DiscountAmount { get; set; }
        public decimal AmountPaid { get; set; }
    }
}