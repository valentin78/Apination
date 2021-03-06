﻿using System;
using System.Collections.Generic;
using System.Linq;
using Sage.Peachtree.API;
using Sage.Peachtree.API.Collections.Generic;

namespace Sage50Connector.API
{
    /// <summary>
    /// Populate from Model Extensions
    /// </summary>
    internal static class Sage50Populations
    {

        public static void PopulateFromModel(this Receipt sageReceipt, Company companyContext, Models.Data.Receipt receipt, SalesInvoice invoice)
        {
            sageReceipt.AccountReference = sageReceipt.AccountReference.PopulateFromModel(receipt.Account, companyContext);

            sageReceipt.DiscountAccountReference = sageReceipt.DiscountAccountReference.PopulateFromModel(receipt.DiscountAccount, companyContext);
            // Если Customer == null, то обязательно
            sageReceipt.MainAddress.PopulateFromModel(receipt.MainAddress);
            sageReceipt.ReferenceNumber = receipt.ReferenceNumber;
            sageReceipt.ReceiptNumber = receipt.ReceiptNumber;
            sageReceipt.DepositTicketID = receipt.DepositTicketID;
            sageReceipt.Date = receipt.Date;
            sageReceipt.PaymentMethod = receipt.PaymentMethod;
            sageReceipt.CreditCardAuthorizationInfo.PopulateFromModel(receipt.CreditCardAuthorizationInfo);

            if (receipt.ApplyToSalesLines != null)
            {
                foreach (var salesLine in receipt.ApplyToSalesLines)
                {
                    var sageSalesLine = sageReceipt.AddSalesLine();
                    sageSalesLine.AccountReference = sageSalesLine.AccountReference.PopulateFromModel(salesLine.Account, companyContext);
                    sageSalesLine.Amount = salesLine.Amount;
                    sageSalesLine.Description = salesLine.Description;
                    sageSalesLine.Quantity = salesLine.Quantity;
                    sageSalesLine.UnitPrice = salesLine.UnitPrice;
                    sageSalesLine.SalesTaxType = salesLine.SalesTaxType;
                }
            }
            if (receipt.ApplyToInvoiceLines != null)
            {
                foreach (var invoiceLine in receipt.ApplyToInvoiceLines)
                {
                    var sageInvoiceLine = sageReceipt.AddInvoiceLine(invoice);
                    sageInvoiceLine.AccountReference = sageInvoiceLine.AccountReference.PopulateFromModel(invoiceLine.Account, companyContext);
                    sageInvoiceLine.Amount = invoiceLine.Amount;
                    sageInvoiceLine.DiscountAmount = invoiceLine.DiscountAmount;
                    sageInvoiceLine.AmountPaid = invoiceLine.AmountPaid;
                    sageInvoiceLine.Description = invoiceLine.Description;
                }
            }
            sageReceipt.Save();
        }

        public static void PopulateFromModel(this Payment sagePayment, Company companyContext, Models.Data.Payment payment)
        {
            sagePayment.AccountReference = sagePayment.AccountReference.PopulateFromModel(payment.Account, companyContext);
            sagePayment.DiscountAccountReference = sagePayment.DiscountAccountReference.PopulateFromModel(payment.DiscountAccount, companyContext);
            // Если Vendor == null, то обязательно
            sagePayment.MainAddress.PopulateFromModel(payment.MainAddress);
            sagePayment.ReferenceNumber = payment.ReferenceNumber;
            sagePayment.Date = payment.Date;
            sagePayment.Memo = payment.Memo;
            sagePayment.PaymentMethod = payment.PaymentMethod;

            foreach (var expenseLine in payment.ApplyToExpenseLines)
            {
                var sageExpnseLine = sagePayment.AddExpenseLine();
                sageExpnseLine.AccountReference = sageExpnseLine.AccountReference.PopulateFromModel(expenseLine.Account, companyContext);
                sageExpnseLine.Amount = expenseLine.Amount;
                sageExpnseLine.Description = expenseLine.Description;
            }
            foreach (var invoiceLine in payment.ApplyToInvoiceLines)
            {
                var sageInvoiceLine = sagePayment.AddInvoiceLine(sagePayment);
                sageInvoiceLine.AccountReference = sageInvoiceLine.AccountReference.PopulateFromModel(invoiceLine.Account, companyContext);
                sageInvoiceLine.Amount = invoiceLine.Amount;
                sageInvoiceLine.Description = invoiceLine.Description;
            }
            sagePayment.Save();
        }

        public static void PopulateFromModel(this SalesInvoice sageInvoice, Company companyContext, Models.Data.SalesInvoice invoice)
        {
            sageInvoice.ReferenceNumber = invoice.ReferenceNumber;

            sageInvoice.Date = invoice.Date;
            sageInvoice.Date = invoice.Date;
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
            sageInvoice.DateDue = invoice.DateDue ?? DateTime.Now;
            sageInvoice.CustomerPurchaseOrderNumber = invoice.CustomerPurchaseOrderNumber;

            foreach (var sageInvoiceApplyToSalesLine in sageInvoice.ApplyToSalesLines)
            {
                sageInvoiceApplyToSalesLine.MarkForDeletion();
            }
            foreach (var salesLine in invoice.SalesLines)
            {
                var sageSalesLine = sageInvoice.AddSalesLine();
                sageSalesLine.Amount = salesLine.Amount;
                sageSalesLine.Quantity = salesLine.Quantity;
                sageSalesLine.SalesTaxType = salesLine.SalesTaxType;
                sageSalesLine.UnitPrice = salesLine.UnitPrice;
                sageSalesLine.Description = salesLine.Description;
                sageSalesLine.AccountReference = sageSalesLine.AccountReference.PopulateFromModel(salesLine.Account, companyContext);
            }

            sageInvoice.FreightAccountReference = sageInvoice.FreightAccountReference.PopulateFromModel(invoice.FreightAccount, companyContext);
            sageInvoice.ShipToAddress.PopulateFromModel(invoice.ShipToAddress);
        }

        public static void PopulateFromModel(this NameAndAddress sageNameAndAddress, Models.Data.NameAndAddress nameAndAddress)
        {
            if (nameAndAddress == null) return;
            sageNameAndAddress.Name = nameAndAddress.Name;
            sageNameAndAddress.Address.PopulateFromModel(nameAndAddress.Address);
        }

        public static void PopulateFromModel(this PhoneNumberCollection sagePhoneNumberCollection, List<Models.Data.PhoneNumber> contactPhoneNumbers)
        {
            if (contactPhoneNumbers == null) return;
            foreach (var contactPhoneNumber in contactPhoneNumbers)
            {
                var phoneKind = contactPhoneNumber.Key.ToEnum<PhoneNumberKind>();
                var contact = sagePhoneNumberCollection.SingleOrDefault(c => c.Key == phoneKind);
                if (contact != null) contact.Number = contactPhoneNumber.Number;
            }
        }

        public static void PopulateFromModel(this Account sageAccount, Models.Data.Account account)
        {
            if (account == null) return;
            sageAccount.ID = account.Id;
            sageAccount.IsInactive = account.IsInactive;
            sageAccount.Classification = account.Classification.ToEnum<AccountClassification>();
            sageAccount.Description = !string.IsNullOrEmpty(account.Description) ? account.Description : account.Classification;
        }

        public static EntityReference<Account> PopulateFromModel(this EntityReference<Account> entityReference, Models.Data.Account account, Company companyContext)
        {
            if (account == null)
                return entityReference;
            EntityList<Account> accList = companyContext.Factories.AccountFactory.List();
            accList.Load();
            var sageCashAccount = accList.FirstOrDefault(i => i.ID == account.Id);
            if (sageCashAccount == null)
            {
                sageCashAccount = companyContext.Factories.AccountFactory.Create();
                sageCashAccount.PopulateFromModel(account);
                sageCashAccount.Save();
            }
            return sageCashAccount.Key;
        }

        public static void PopulateFromModel(this CreditCardAuthorizationInfo sageCardAuthInfo, Models.Data.CreditCardAuthorizationInfo cardAuthInfo)
        {
            if (cardAuthInfo == null) return;

            sageCardAuthInfo.Address.PopulateFromModel(cardAuthInfo.Address);
            sageCardAuthInfo.AuthorizationCode = cardAuthInfo.AuthorizationCode;
            sageCardAuthInfo.LastFourDigits = cardAuthInfo.AuthorizationCode;
            sageCardAuthInfo.ExpirationDate = cardAuthInfo.ExpirationDate;
            sageCardAuthInfo.Note = cardAuthInfo.Note;
            sageCardAuthInfo.AmountAuthorized = cardAuthInfo.AmountAuthorized;
        }

        public static void PopulateFromModel(this Address sageAddress, Models.Data.Address address)
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

        public static void PopulateFromModel(this Contact sageContact, Models.Data.Contact contact)
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
        public static void PopulateFromModel(this Customer sageCustomer, Company companyContext, Models.Data.Customer customer)
        {
            if (customer == null) return;

            sageCustomer.ID = customer.ExternalId;
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

            sageCustomer.ShipToContact.PopulateFromModel(customer.ShipToContact);
            sageCustomer.BillToContact.PopulateFromModel(customer.BillToContact);

            sageCustomer.PhoneNumbers.PopulateFromModel(customer.PhoneNumbers);

            sageCustomer.CashAccountReference = sageCustomer.CashAccountReference.PopulateFromModel(customer.CashAccount, companyContext);
            sageCustomer.UsualSalesAccountReference = sageCustomer.UsualSalesAccountReference.PopulateFromModel(customer.UsualSalesAccount, companyContext);
        }

        public static void PopulateFromModel(this Vendor sageVendor, Company companyContext, Models.Data.Vendor vendor)
        {
            if (vendor == null) return;

            sageVendor.ID = vendor.ExternalId;
            sageVendor.TaxIDNumber = vendor.TaxIDNumber;
            sageVendor.Form1099Type = vendor.Form1099Type.ToEnum<VendorForm1099Type>();
            sageVendor.UsingPaymentDefaults = vendor.UsingPaymentDefaults;
            sageVendor.VendorSince = vendor.VendorSince;

            sageVendor.ExpenseAccountReference = sageVendor.ExpenseAccountReference.PopulateFromModel(vendor.ExpenseAccount, companyContext);
            sageVendor.ShipmentsContact.PopulateFromModel(vendor.ShipmentsContact);
            sageVendor.PurchaseOrdersContact.PopulateFromModel(vendor.PurchaseOrdersContact);
            sageVendor.PaymentsContact.PopulateFromModel(vendor.PaymentsContact);

            sageVendor.Name = vendor.Name;
            sageVendor.IsInactive = vendor.IsInactive;
            sageVendor.AccountNumber = vendor.AccountNumber;
            sageVendor.ReplaceInventoryItemIDWithPartNumber = vendor.ReplaceInventoryItemIDWithPartNumber;
            sageVendor.ReplaceInventoryItemIDWithUPC = vendor.ReplaceInventoryItemIDWithUPC;
            sageVendor.IncludePurchaseRepresentativeOnEmailedForms = vendor.IncludePurchaseRepresentativeOnEmailedForms;
            sageVendor.ShipVia = vendor.ShipVia;
            sageVendor.WebSiteURL = vendor.WebSiteURL;
            sageVendor.UseEmailToDeliverForms = vendor.UseEmailToDeliverForms;
            sageVendor.Email = vendor.Email;
            sageVendor.Category = vendor.Category;

            sageVendor.PhoneNumbers.PopulateFromModel(vendor.PhoneNumbers);

            sageVendor.CashAccountReference = sageVendor.CashAccountReference.PopulateFromModel(vendor.CashAccount, companyContext);
        }
    }
}

