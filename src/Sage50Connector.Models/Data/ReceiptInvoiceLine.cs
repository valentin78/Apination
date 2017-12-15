namespace Sage50Connector.Models.Data
{
    /// <summary>
    /// Class name and structure correspond Sage50 SDK
    /// </summary>
    public class ReceiptInvoiceLine : TransactionLine
    {
        public decimal DiscountAmount { get; set; }
        public decimal AmountPaid { get; set; }
    }
}