﻿namespace Sage50Connector.Models.Data
{
    public class ReceiptInvoiceLine : TransactionLine
    {
        public decimal Quantity { get; set; }
        public int SalesTaxType { get; set; }
        public decimal UnitPrice { get; set; }
    }
}