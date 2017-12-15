namespace Sage50Connector.Models.Data
{
    /// <summary>
    /// Class name and structure correspond Sage50 SDK
    /// </summary>
    public class ReceiptSalesLine : TransactionLine
    {
        public decimal Quantity { get; set; }
        public int SalesTaxType { get; set; }
        public decimal UnitPrice { get; set; }
    }
}