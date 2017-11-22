namespace Sage50Connector.Models.Payloads
{
    public class SalesInvoiceSalesLine: TransactionLine
    {
        public decimal Quantity { get; set; }
        public int SalesTaxType { get; set; }
        public decimal UnitPrice { get; set; }
    }
}