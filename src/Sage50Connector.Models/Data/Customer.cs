using System;
using System.Collections.Generic;

//using Sage.Peachtree.API;

// ReSharper disable InconsistentNaming

namespace Sage50Connector.Models.Data
{
    public class Customer
    {
        public Customer()
        {
            CreditStatus = "NoLimit";
        }

        public string ExternalId { get; set; }
        public string GlobalKey(string source) => $"{source} : {ExternalId}";

        public bool IsInactive { get; set; }
        public bool IsProspect { get; set; }
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
        public List<PhoneNumber> PhoneNumbers { get; set; }
        public string Email { get; set; }
        public Contact ShipToContact { get; set; }
        public Contact BillToContact { get; set; }

        public string Category { get; set; }
        /// <summary>
        /// CustomerCreditStatus enum
        /// </summary>
        public string CreditStatus { get; set; }
        public DateTime? CustomerSince { get; set; }

        // TODO: unknown purpose
        //public CustomFieldValueCollection CustomFieldValues { get; }

        // TODO: skip because all public properties in structure readonly
        //public string Terms { get; set; }

        // TODO: factory not support create() 
        //public List<Contact> Contacts { get; set; }
        //public Employee SalesRepresentative { get; set; }
    }
}