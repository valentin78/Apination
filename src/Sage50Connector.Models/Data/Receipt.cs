using System.Collections.Generic;

namespace Sage50Connector.Models.Data
{
    /// <summary>
    /// Represents Receive Money from Customer card data
    /// Class name and structure correspond Sage50 SDK
    /// </summary>
    public class Receipt : Transaction
    {
        public List<ReceiptSalesLine> ApplyToSalesLines { get; set; }
        public CreditCardAuthorizationInfo CreditCardAuthorizationInfo { get; set; }
        public NameAndAddress MainAddress { get; set; }
        public string ReceiptNumber { get; set; }
        public Account DiscountAccount { get; set; }
        public string PaymentMethod { get; set; }
        public List<ReceiptInvoiceLine> ApplyToInvoiceLines { get; set; }
        public Customer Customer { get; set; }
        public string DepositTicketID { get; set; }
    }
}