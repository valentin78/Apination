using System;
using System.Collections.Generic;
// ReSharper disable InconsistentNaming

namespace Sage50Connector.Models.Data
{
    public class Vendor
    {
        public string GlobalKey(string source) => $"{source} : {ExternalId}";

        public bool ReplaceInventoryItemIDWithUPC { get; set; }
        public string PaymentMethod { get; set; }
        public bool ReplaceInventoryItemIDWithPartNumber { get; set; }
        public List <PhoneNumber> PhoneNumbers { get; set; }
        public string ShipVia { get; set; }
        public string TaxIDNumber { get; set; }
        public string Form1099Type { get; set; }
        public bool UseEmailToDeliverForms { get; set; }
        public bool UsingPaymentDefaults { get; set; }
        public DateTime? VendorSince { get; set; }
        public string WebSiteURL { get; set; }
        public List<Contact> Contacts { get; set; }
        public Account ExpenseAccount { get; set; }
        public string Name { get; set; }
        public Account CashAccount { get; set; }
        public Contact ShipmentsContact { get; set; }
        public Contact PurchaseOrdersContact { get; set; }
        public Contact PaymentsContact { get; set; }
        public string Category { get; set; }
        public string Email { get; set; }
        public string ExternalId { get; set; }
        public bool IncludePurchaseRepresentativeOnEmailedForms { get; set; }
        public bool IsInactive { get; set; }
        public string AccountNumber { get; set; }

    }
}
