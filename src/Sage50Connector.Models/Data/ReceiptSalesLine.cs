namespace Sage50Connector.Models.Data
{
    public class ReceiptSalesLine : TransactionLine
    {
        //public EntityReference InventoryItemReference { get; set; }
        //public Job Job { get; set; }
        public decimal Quantity { get; set; }
        public int SalesTaxType { get; set; }
        public decimal UnitPrice { get; set; }
    }
}