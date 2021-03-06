﻿using System.Collections.Generic;

namespace Sage50Connector.Models.Data
{
    public class Payment : Transaction
    {
        public Vendor Vendor { get; set; }
        public Account DiscountAccount { get; set; }
        public NameAndAddress MainAddress { get; set; }
        public List<PaymentInvoiceLine> ApplyToInvoiceLines { get; set; }
        public List<PaymentExpenseLine> ApplyToExpenseLines { get; set; }
        public string Memo { get; set; }
        public string PaymentMethod { get; set; }
    }
}