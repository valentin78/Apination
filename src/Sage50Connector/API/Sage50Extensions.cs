using System;
using System.Linq;
using Sage.Peachtree.API;
using Sage.Peachtree.API.Collections.Generic;

using Customer = Sage50Connector.Models.Payloads.Customer;
using Contact = Sage50Connector.Models.Payloads.Contact;
using Address = Sage50Connector.Models.Payloads.Address;
using Account = Sage50Connector.Models.Payloads.Account;

using SageCustomer = Sage.Peachtree.API.Customer;
using SageContact = Sage.Peachtree.API.Contact;
using SageAddress = Sage.Peachtree.API.Address;
using SageAccount= Sage.Peachtree.API.Account;

namespace Sage50Connector.API
{
    internal static class Sage50Extensions
    {
        public static void PopulateFromModel(this SageAccount sageAccount, Account account)
        {
            sageAccount.Description = account.Description;
            sageAccount.ID = account.Id;
            sageAccount.IsInactive = account.IsInactive;
            sageAccount.Classification = account.Classification.ToEnum<AccountClassification>();
        }

        public static void PopulateFromModel(this SageAddress sageAddress, Address address)
        {
            sageAddress.Address1 = address.Address1;
            sageAddress.Address2 = address.Address2;
            sageAddress.City = address.City;
            sageAddress.State = address.State;
            sageAddress.Zip = address.Zip;
            sageAddress.Country = address.Country;
            sageAddress.SalesTaxCode = address.SalesTaxCode;
        }

        public static void PopulateFromModel(this SageContact sageContact, Company company, Contact contact)
        {
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
            //sageContact.PhoneNumbers = contact.PhoneNumbers;
        }

        public static void PopulateFromModel(this SageCustomer sageCustomer, Company company, Customer customer)
        {
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

            // TODO: доработать
            //sageCustomer.Contacts
            //sageCustomer.PhoneNumbers

            var cashAccount = sageCustomer.CashAccountReference.Load(company);
            cashAccount.PopulateFromModel(customer.CashAccount);

            var usualSalesAccount = sageCustomer.UsualSalesAccountReference.Load(company);
            usualSalesAccount.PopulateFromModel(customer.UsualSalesAccount);
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

