using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Sage.Peachtree.API;
using Sage.Peachtree.API.Collections.Generic;

namespace Sage50Connector.API
{
    internal static class Sage50Extensions
    {
        public static void PopulateFromModel(this SalesInvoice sageInvoice, Company company, Models.Payloads.SalesInvoice invoice)
        {
            sageInvoice.ShipDate = invoice.ShipDate;
            sageInvoice.CustomerNote = invoice.CustomerNote;

            sageInvoice.InternalNote = invoice.InternalNote;
            sageInvoice.TermsDescription = invoice.TermsDescription;
            sageInvoice.StatementNote = invoice.StatementNote;
            sageInvoice.ShipVia = invoice.ShipVia;
            sageInvoice.PrintCustomerNoteAfterLineItems = invoice.PrintCustomerNoteAfterLineItems;
            sageInvoice.FreightAmount = invoice.FreightAmount;
            sageInvoice.DropShip = invoice.DropShip;
            sageInvoice.DiscountDate = invoice.DiscountDate;
            sageInvoice.DiscountAmount = invoice.DiscountAmount;
            sageInvoice.DateDue = invoice.DateDue;
            sageInvoice.CustomerPurchaseOrderNumber = invoice.CustomerPurchaseOrderNumber;

            sageInvoice.FreightAccountReference = sageInvoice.FreightAccountReference.PopulateFromModel(invoice.FreightAccount, company);
            sageInvoice.ShipToAddress.PopulateFromModel(invoice.ShipToAddress);
        }

        public static void PopulateFromModel(this NameAndAddress sageNameAndAddress, Models.Payloads.NameAndAddress nameAndAddress)
        {
            sageNameAndAddress.Name = nameAndAddress.Name;
            sageNameAndAddress.Address.PopulateFromModel(nameAndAddress.Address);
        }

        public static void PopulateFromModel(this PhoneNumber sagePhoneNumber, Models.Payloads.PhoneNumber phoneNumber)
        {
            sagePhoneNumber.Number = phoneNumber.Number;
        }

        public static void PopulateFromModel(this PhoneNumberCollection sagePhoneNumberCollection, List<Models.Payloads.PhoneNumber> contactPhoneNumbers)
        {
            if (contactPhoneNumbers == null) return;
            foreach (var contactPhoneNumber in contactPhoneNumbers)
            {
                var contact = contactPhoneNumbers.SingleOrDefault(c => c.Key == contactPhoneNumber.Key);
                if (contact != null) contactPhoneNumber.Number = contact.Number;
            }
        }

        public static void PopulateFromModel(this Account sageAccount, Models.Payloads.Account account)
        {
            if (account == null) return;
            sageAccount.Description = account.Description;
            sageAccount.ID = account.Id;
            sageAccount.IsInactive = account.IsInactive;
            sageAccount.Classification = account.Classification.ToEnum<AccountClassification>();
        }

        public static EntityReference<Account> PopulateFromModel(this EntityReference<Account> entityReference, Models.Payloads.Account account, Company company)
        {
            if (account == null) return entityReference;

            if (entityReference.IsEmpty)
            {
                var sageCashAccount = company.Factories.AccountFactory.Create();
                sageCashAccount.PopulateFromModel(account);
                sageCashAccount.Save();
                return sageCashAccount.Key;
            }

            var cashAccount = entityReference.Load(company);
            cashAccount.PopulateFromModel(account);
            return entityReference;
        }

        public static void PopulateFromModel(this Address sageAddress, Models.Payloads.Address address)
        {
            if (address == null) return;
            sageAddress.Address1 = address.Address1;
            sageAddress.Address2 = address.Address2;
            sageAddress.City = address.City;
            sageAddress.State = address.State;
            sageAddress.Zip = address.Zip;
            sageAddress.Country = address.Country;
            sageAddress.SalesTaxCode = address.SalesTaxCode;
        }

        public static void PopulateFromModel(this Contact sageContact, Company company, Models.Payloads.Contact contact)
        {
            if (contact == null) return;
            sageContact.FirstName = contact.FirstName;
            sageContact.MiddleInitial = contact.MiddleInitial;
            sageContact.LastName = contact.LastName;
            sageContact.CompanyName = contact.CompanyName;
            sageContact.Gender = contact.Gender.ToEnum<Gender>();

            sageContact.Address.PopulateFromModel(contact.Address);

            sageContact.Suffix = contact.Suffix;
            sageContact.Prefix = contact.Prefix;
            sageContact.Notes = contact.Notes;
            sageContact.NameToUseOnForms = contact.NameToUseOnForms.ToEnum<NameToUseOnForms>();

            sageContact.Title = contact.Title;
            sageContact.Email = contact.Email;

            sageContact.PhoneNumbers.PopulateFromModel(contact.PhoneNumbers);
        }

        public static void PopulateFromModel(this Customer sageCustomer, Company company, Models.Payloads.Customer customer)
        {
            if (customer == null) return;

            Debugger.Launch();

            sageCustomer.ID = customer.Id;
            sageCustomer.Name = customer.Name;
            sageCustomer.IsInactive = customer.IsInactive;
            sageCustomer.IsProspect = customer.IsProspect;
            sageCustomer.AccountNumber = customer.AccountNumber;
            sageCustomer.OpenPurchaseOrderNumber = customer.OpenPurchaseOrderNumber;
            sageCustomer.PriceLevel = customer.PriceLevel;
            sageCustomer.ReplaceInventoryItemIDWithPartNumber = customer.ReplaceInventoryItemIDWithPartNumber;
            sageCustomer.ReplaceInventoryItemIDWithUPC = customer.ReplaceInventoryItemIDWithUPC;
            sageCustomer.ResaleNumber = customer.ResaleNumber;
            sageCustomer.ShipVia = customer.ShipVia;
            sageCustomer.WebSiteURL = customer.WebSiteURL;
            sageCustomer.UseEmailToDeliverForms = customer.UseEmailToDeliverForms;
            sageCustomer.Email = customer.Email;
            sageCustomer.Category = customer.Category;
            sageCustomer.CreditStatus = customer.CreditStatus.ToEnum<CustomerCreditStatus>();
            sageCustomer.CustomerSince = customer.CustomerSince;

            sageCustomer.ShipToContact.PopulateFromModel(company, customer.ShipToContact);
            sageCustomer.BillToContact.PopulateFromModel(company, customer.BillToContact);

            sageCustomer.PhoneNumbers.PopulateFromModel(customer.PhoneNumbers);

            sageCustomer.CashAccountReference = sageCustomer.CashAccountReference.PopulateFromModel(customer.CashAccount, company);
            sageCustomer.UsualSalesAccountReference = sageCustomer.UsualSalesAccountReference.PopulateFromModel(customer.UsualSalesAccount, company);
        }

        /// <summary>
        /// extension method convert string representation of enum value to appropriate enum value
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static TEnum ToEnum<TEnum>(this string value) where TEnum : struct
        {
            if (!typeof(TEnum).IsEnum) throw new ArgumentException("Invalid generic type, must me enum", nameof(TEnum));

            if (Enum.TryParse(value, out TEnum result)) return result;

            throw new ArgumentException($"Can not convert value: '{value}' to enum type: '{typeof(TEnum)}'");
        }

        /// <summary>
        /// apply filter on EntityList by ID and load single or default item from list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="id"></param>
        public static T SingleOrDefault<T>(this EntityList<T> list, object id) where T : Entity
        {
            FilterExpression expression = FilterExpression.Equal(
                FilterExpression.Property($"{typeof(T).Name}.ID"),
                FilterExpression.Constant(id.ToString()));

            var modifier = LoadModifiers.Create();
            modifier.Filters = expression;
            list.Load(modifier);

            return list.SingleOrDefault();
        }
    }
}

