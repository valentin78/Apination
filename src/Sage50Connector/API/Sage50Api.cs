using System;
using System.Linq;
using Sage.Peachtree.API;
using Sage.Peachtree.API.Collections.Generic;
using Sage50Connector.Core;
using Sage50Connector.Models.Payloads;

namespace Sage50Connector.API
{
    /// <summary>
    /// Sage50 SDK wrapper
    /// </summary>
    public class Sage50Api : IDisposable
    {
        // ReSharper disable once InconsistentNaming
        private readonly LocalDbApi localDbApi = new LocalDbApi();

        // ReSharper disable once InconsistentNaming
        private PeachtreeSession ApiSession;

        private readonly string actionSource;

        public Sage50Api(string actionSource)
        {
            this.actionSource = actionSource;
        }

        // ReSharper disable once InconsistentNaming
        private Company CompanyContext { get; set; }

        private PeachtreeSession CurrentSession
        {
            get
            {
                // if the session has not been initialized
                if (ApiSession != null && ApiSession.SessionActive) return ApiSession;

                // dispose of the inactive session
                ApiSession?.Dispose();

                // create a new session instance
                ApiSession = new PeachtreeSession();

                // start the session.  
                // with no application ID, you can only open Sample companies
                ApiSession.Begin(ApplicationConfig.Sage50ApplicationID);
                // return the current session
                return ApiSession;
            }
        }

        public void OpenCompany(string companyName)
        {
            CloseCurrentCompany();

            var companyId = FindCompany(companyName);

            // Ask the Sage 50 application if this application has been granted access to the company.
            var authResult = CurrentSession.VerifyAccess(companyId);

            // if the app has never asked for authorization before, We need to ask now
            if (authResult == AuthorizationResult.NoCredentials)
            {
                authResult = CurrentSession.RequestAccess(companyId);
            }

            // handle the authorization result
            switch (authResult)
            {
                case AuthorizationResult.Granted:
                    // open the company
                    CompanyContext = CurrentSession.Open(companyId);
                    break;
                default:
                    throw new ArgumentException($"Can not open company {companyName}. Authorization Result: {authResult}");
            }
        }

        public void Dispose()
        {
            CloseCurrentCompany();
            ApiSession?.Dispose();
        }

        private void CloseCurrentCompany()
        {
            if (CompanyContext == null) return;

            CompanyContext.Close();
            CompanyContext = null;
        }

        private CompanyIdentifierList CompaniesList()
        {
            return CurrentSession.CompanyList(CurrentSession.Configuration.ServerName);
        }

        private CompanyIdentifier FindCompany(string companyName)
        {
            var result = CompaniesList().SingleOrDefault(c => c.CompanyName == companyName);
            if (result == null) throw new ArgumentException($"Can not find company with name: \"{companyName}\"");
            return result;
        }

        private CustomerList CustomersList()
        {
            if (CompanyContext == null) throw new ArgumentException("Company must be open before");
            return CompanyContext.Factories.CustomerFactory.List();
        }

        private VendorList VendorsList()
        {
            if (CompanyContext == null) throw new ArgumentException("Company must be open before");
            return CompanyContext.Factories.VendorFactory.List();
        }

        private SalesInvoiceList SalesInvoicesList()
        {
            if (CompanyContext == null) throw new ArgumentException("Company must be open before");
            return CompanyContext.Factories.SalesInvoiceFactory.List();
        }

        public EntityReference<Customer> CreateOrUpdateCustomer(Models.Data.Customer customer)
        {
            var sageCustomer = FindSageCustomer(customer) ?? CompanyContext.Factories.CustomerFactory.Create();

            sageCustomer.PopulateFromModel(CompanyContext, customer);
            sageCustomer.Save();

            return sageCustomer.Key;
        }

        public void CreateInvoice(Models.Data.SalesInvoice invoice)
        {
            var customer = FindSageCustomer(invoice.Customer);

            SalesInvoice sageInvoice;
            if (customer != null)
            {
                sageInvoice = FindInvoice(invoice.ReferenceNumber, customer.ID);
                if (sageInvoice != null)
                    throw new MessageException(
                        $"Found invoice with ReferenceNumber: '{invoice.ReferenceNumber}' and CustomerId: '{customer.ID}'. Transaction aborted.");
            }

            // if no exist invoice, we can create new
            sageInvoice = CompanyContext.Factories.SalesInvoiceFactory.Create();

            sageInvoice.CustomerReference = CreateOrUpdateCustomer(invoice.Customer);

            sageInvoice.PopulateFromModel(CompanyContext, invoice);
            sageInvoice.Save();
        }

        public void UpdateInvoice(Models.Data.SalesInvoice invoice)
        {
            var customer = FindSageCustomer(invoice.Customer);

            if (customer == null)
                throw new MessageException($"Not found customer with Key: '{invoice.Customer.GlobalKey(actionSource)}'. Transaction aborted.");

            var sageInvoice = FindInvoice(invoice.ReferenceNumber, customer.ID);
            if (sageInvoice == null)
                throw new MessageException($"Not found invoice with ReferenceNumber: '{invoice.ReferenceNumber}' and CustomerId: '{customer.ID}'. Transaction aborted.");

            sageInvoice.CustomerReference = CreateOrUpdateCustomer(invoice.Customer);

            sageInvoice.PopulateFromModel(CompanyContext, invoice);
            sageInvoice.Save();
        }

        /// <summary>
        /// find invoice by reference number and customer id
        /// </summary>
        private SalesInvoice FindInvoice(string referenceNumber, string customerId)
        {
            // load invoices list by ReferenceNumber 
            var invoices = SalesInvoicesList().FilterBy("ReferenceNumber", referenceNumber);
            return invoices.SingleOrDefault(i =>
            {
                // load Customer
                var customer = i.CustomerReference.Load(CompanyContext);
                return customer.ID == customerId;
            });
        }

        private Customer FindSageCustomer(Models.Data.Customer customer)
        {
            var customerKey = customer.GlobalKey(actionSource);
            var sageCustomers = CustomersList();

            // find mapping for customer globalKey
            var sageCustomerId = localDbApi.GetCustomerIdByKey(customerKey);
            if (sageCustomerId != null) return sageCustomers.SingleOrDefault(sageCustomerId);

            // find by GlobalKey in Sage50
            var sageCustomer = sageCustomers.SingleOrDefault(customerKey);
            if (sageCustomer != null)
            {
                localDbApi.StoreCustomerId(customerKey, sageCustomer.ID);
                return sageCustomer;
            }

            // if no mapping found and by extrnalId, find customer by email, phone or name and store mapping to localDb
            if (string.IsNullOrEmpty(customer.Email) && !customer.PhoneNumbers.IsPhonesAbsent())
            {
                // if email empty and phone present, load whole customers list and find first by phone
                sageCustomers.Load();
                sageCustomer = sageCustomers.FirstOrDefault(c => c.PhoneNumbers.ContainsOneOf(customer.PhoneNumbers));
            }
            if (sageCustomer == null)
            {
                FilterExpression expression;
                if (!string.IsNullOrEmpty(customer.Email))
                {
                    expression = FilterExpression.Equal(FilterExpression.Property("Customer.Email"),
                        FilterExpression.Constant(customer.Email));
                } else if (!string.IsNullOrEmpty(customer.Name))
                {
                    expression = FilterExpression.Equal(FilterExpression.Property("Customer.Name"),
                        FilterExpression.Constant(customer.Name));
                }
                else
                {
                    throw new MessageException("Can not search customer because name and email is null");
                }

                var modifier = LoadModifiers.Create();
                modifier.Filters = expression;
                sageCustomers.Load(modifier);

                if (sageCustomers.Count == 0) return null;

                if (sageCustomers.Count > 1)
                    throw new MessageException(
                        $"Found more that one customer with name: '{customer.Name}' or phones or email: '{customer.Email}'");

                sageCustomer = sageCustomers.First();
            }

            if (sageCustomer == null) return null;

            sageCustomer = sageCustomers.First();
            localDbApi.StoreCustomerId(customerKey, sageCustomer.ID);
            return sageCustomer;
        }

        private Vendor FindSageVendor(Models.Data.Vendor vendor)
        {
            var vendorKey = vendor.GlobalKey(actionSource);
            var sageVendors = VendorsList();

            // find mapping for vendor globalKey
            var sageVendorId = localDbApi.GetVendorIdByKey(vendorKey);
            if (sageVendorId != null) return sageVendors.SingleOrDefault(sageVendorId);

            // find by GlobalKey in Sage50
            var sageCustomer = sageVendors.SingleOrDefault(vendorKey);
            if (sageCustomer != null)
            {
                localDbApi.StoreVendorrId(vendorKey, sageCustomer.ID);
                return sageCustomer;
            }

            // if no mapping found and by extrnalId, find customer by name or email and store mapping to localDb
            FilterExpression expression;
            if (!string.IsNullOrEmpty(vendor.Name) && !string.IsNullOrEmpty(vendor.Email))
            {
                expression = FilterExpression.AndAlso(
                    FilterExpression.Equal(FilterExpression.Property("Vendor.Name"), FilterExpression.Constant(vendor.Name)),
                    FilterExpression.Equal(FilterExpression.Property("Vendor.Email"), FilterExpression.Constant(vendor.Email))
                );
            }
            else if (!string.IsNullOrEmpty(vendor.Name))
            {
                expression = FilterExpression.Equal(FilterExpression.Property("Vendor.Name"), FilterExpression.Constant(vendor.Name));
            }
            else if (!string.IsNullOrEmpty(vendor.Email))
            {
                expression = FilterExpression.Equal(FilterExpression.Property("Vendor.Email"),
                    FilterExpression.Constant(vendor.Email));
            }
            else
            {
                throw new MessageException("Can not find Vendor because name and email is null");
            }

            var modifier = LoadModifiers.Create();
            modifier.Filters = expression;
            sageVendors.Load(modifier);

            if (sageVendors.Count == 0) return null;

            if (sageVendors.Count > 1)
                throw new MessageException($"Found more that one vendor with name: '{vendor.Name}' or email: '{vendor.Email}'");

            sageCustomer = sageVendors.First();
            localDbApi.StoreVendorrId(vendorKey, sageCustomer.ID);
            return sageCustomer;
        }

        /// <summary>
        /// Find or Create and after that Populate Vendor data
        /// </summary>
        /// <param name="vendor"></param>
        /// <returns></returns>
        private EntityReference<Vendor> UpsertVendor(Models.Data.Vendor vendor)
        {
            var sageVendor = FindSageVendor(vendor) ?? CompanyContext.Factories.VendorFactory.Create();
            sageVendor.PopulateFromModel(CompanyContext, vendor);
            sageVendor.Save();
            return sageVendor.Key;
        }

        private void UpsertInvoice(Models.Data.SalesInvoice invoice)
        {
            var customer = FindSageCustomer(invoice.Customer);
            // if no exist Customer, goto CreateInvoice
            if (customer == null) CreateInvoice(invoice);
            else
            {
                var sageInvoice = FindInvoice(invoice.ReferenceNumber, customer.ID);
                if (sageInvoice == null) CreateInvoice(invoice);
                else UpdateInvoice(invoice);
            }
        }

        public void CreatePayment(PaymentPayload paymentPayload)
        {
            UpsertInvoice(paymentPayload.invoice);

            foreach (var payment in paymentPayload.payments)
            {
                var sagePayment = CompanyContext.Factories.PaymentFactory.Create();

                sagePayment.VendorReference = UpsertVendor(payment.Vendor);

                sagePayment.PopulateFromModel(CompanyContext, payment);
            }
        }
        public void CreateReceipt (ReceiveAndApplyMoneyPayload payload)
        {
            foreach (var receipt in payload.receipts)
            {
                var sagerReceipt = CompanyContext.Factories.ReceiptFactory.Create();

                sagerReceipt.CustomerReference = CreateOrUpdateCustomer(receipt.Customer);

                sagerReceipt.PopulateFromModel(CompanyContext, receipt);
            }
        }
    }
}
