using System;
using System.Collections.Generic;

//using Sage.Peachtree.API;

// ReSharper disable InconsistentNaming

namespace Sage50Connector.Models.Payloads
{
    public class Customer
    {
        public Customer()
        {
            BillToContact = new Contact();
        }
        public bool IsInactive { get; set; }
        public bool IsProspect { get; set; }
        // TODO replace SAGE classes
        //public PaymentTerms Terms { get; set; }
        public string Id { get; set; }
        public string OpenPurchaseOrderNumber { get; set; }
        public string PaymentMethod { get; set; }
        public short PriceLevel { get; set; }
        public bool ReplaceInventoryItemIDWithPartNumber { get; set; }
        public bool ReplaceInventoryItemIDWithUPC { get; set; }
        public string ResaleNumber { get; set; }
        public string ShipVia { get; set; }
        public string WebSiteURL { get; set; }
        public string AccountNumber { get; set; }
        public Account CashAccountReference { get; set; }
        public bool UseEmailToDeliverForms { get; set; }
        public string Name { get; set; }
        public Account UsualSalesAccountReference { get; set; }
        //public Employee SalesRepresentativeReference { get; set; }
        public List<PhoneNumber> PhoneNumbers { get; set; }
        public string Email { get; set; }
        //public CustomFieldValueCollection CustomFieldValues { get; }
        public Contact ShipToContact { get; set; }
        public Contact BillToContact { get; set; }

        public string Category { get; set; }
        //TODO: replace SAGE classes
        //public CustomerCreditStatus CreditStatus { get; set; }
        public DateTime? CustomerSince { get; set; }
        public List<Contact> Contacts { get; set; }
    }

    public class PhoneNumber
    {
        public string Number { get; set; }
        // TODO replace SAGE enums
        //public PhoneNumberKind Key { get; }
    }
}