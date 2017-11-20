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
            CashAccount = new Account();
            UsualSalesAccount = new Account();
            PhoneNumbers = new List<PhoneNumber>();
            ShipToContact = new Contact();
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
        public Account CashAccount { get; set; }
        public bool UseEmailToDeliverForms { get; set; }
        public string Name { get; set; }
        public Account UsualSalesAccount { get; set; }
        //public Employee SalesRepresentativeReference { get; set; }
        public List<PhoneNumber> PhoneNumbers { get; set; }
        public string Email { get; set; }
        //public CustomFieldValueCollection CustomFieldValues { get; }
        public Contact ShipToContact { get; set; }
        public Contact BillToContact { get; set; }

        public string Category { get; set; }
        /// <summary>
        /// CustomerCreditStatus enum
        /// </summary>
        public string CreditStatus { get; set; }
        public DateTime? CustomerSince { get; set; }
        public List<Contact> Contacts { get; set; }
    }
}