using System.Collections.Generic;

namespace Sage50Connector.Models.Data
{
    public class Receipt : Transaction
    {
        public List<ReceiptSalesLine> ApplyToSalesLines { get; set; }
        public CreditCardAuthorizationInfo CreditCardAuthorizationInfo { get; }
        public NameAndAddress MainAddress { get; }
        public string ReceiptNumber { get; set; }
        public Account DiscountAccount { get; set; }
        public string PaymentMethod { get; set; }
        public List<ReceiptInvoiceLine> ApplyToInvoiceLines { get; set; }
        public Customer Customer { get; set; }
        public string DepositTicketID { get; set; }
    }
}