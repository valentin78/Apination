using System;
using System.Linq;
using log4net;
using Sage.Peachtree.API;
using Sage.Peachtree.API.Collections.Generic;
using Sage50Connector.Core;
using Sage50Connector.Models.Payloads;
using Payment = Sage50Connector.Models.Data.Payment;

namespace Sage50Connector.API
{
    /// <summary>
    /// Sage50 SDK wrapper
    /// </summary>
    public class Sage50Api : IDisposable
    {
        public static readonly ILog Log = LogManager.GetLogger(typeof(Sage50Api));

        // ReSharper disable once InconsistentNaming
        private readonly LocalDbApi localDbApi = new LocalDbApi();

        // ReSharper disable once InconsistentNaming
        private PeachtreeSession ApiSession;

        private readonly string _actionSource;

        public Sage50Api(string actionSource)
        {
            _actionSource = actionSource;
        }

        // ReSharper disable once InconsistentNaming
        public Company CompanyContext { get; set; }

        protected PeachtreeSession CurrentSession
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

        public CompanyIdentifierList CompaniesList()
        {
            return CurrentSession.CompanyList(CurrentSession.Configuration.ServerName);
        }

        public CompanyIdentifier FindCompany(string companyName)
        {
            var result = CompaniesList().SingleOrDefault(c => c.CompanyName == companyName);
            if (result == null) throw new ArgumentException($"Can not find company by name: \"{companyName}\"");
            return result;
        }

        public CustomerList CustomersList()
        {
            if (CompanyContext == null) throw new ArgumentException("Company must be open before");
            return CompanyContext.Factories.CustomerFactory.List();
        }

        public VendorList VendorsList()
        {
            if (CompanyContext == null) throw new ArgumentException("Company must be open before");
            return CompanyContext.Factories.VendorFactory.List();
        }

        public SalesInvoiceList SalesInvoicesList()
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
                throw new MessageException($"Not found customer with Key: '{invoice.Customer.GlobalKey(_actionSource)}'. Transaction aborted.");

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
            var customerKey = customer.GlobalKey(_actionSource);
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

            // if no mapping found and by extrnalId, find customer by name or email and store mapping to localDb
            FilterExpression expression;
            if (!string.IsNullOrEmpty(customer.Name) && !string.IsNullOrEmpty(customer.Email))
            {
                expression = FilterExpression.AndAlso(
                    FilterExpression.Equal(FilterExpression.Property("Customer.Name"), FilterExpression.Constant(customer.Name)),
                    FilterExpression.Equal(FilterExpression.Property("Customer.Email"), FilterExpression.Constant(customer.Email))
                );
            }
            else if (!string.IsNullOrEmpty(customer.Name))
            {
                expression = FilterExpression.Equal(FilterExpression.Property("Customer.Name"), FilterExpression.Constant(customer.Name));
            }
            else if (!string.IsNullOrEmpty(customer.Email))
            {
                expression = FilterExpression.Equal(FilterExpression.Property("Customer.Email"),
                    FilterExpression.Constant(customer.Email));
            }
            else
            {
                throw new MessageException($"Can not find customer because name and email is null");
            }

            var modifier = LoadModifiers.Create();
            modifier.Filters = expression;
            sageCustomers.Load(modifier);

            if (sageCustomers.Count == 0) return null;

            if (sageCustomers.Count > 1)
                throw new MessageException($"Found more that one customer by name: '{customer.Name}' or email: '{customer.Email}'");

            sageCustomer = sageCustomers.First();
            localDbApi.StoreCustomerId(customerKey, sageCustomer.ID);
            return sageCustomer;
        }

        private Vendor FindSageVendor(Models.Data.Vendor vendor)
        {
            var vendorKey = vendor.GlobalKey(_actionSource);
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
                    FilterExpression.Equal(FilterExpression.Property("Customer.Name"), FilterExpression.Constant(vendor.Name)),
                    FilterExpression.Equal(FilterExpression.Property("Customer.Email"), FilterExpression.Constant(vendor.Email))
                );
            }
            else if (!string.IsNullOrEmpty(vendor.Name))
            {
                expression = FilterExpression.Equal(FilterExpression.Property("Customer.Name"), FilterExpression.Constant(vendor.Name));
            }
            else if (!string.IsNullOrEmpty(vendor.Email))
            {
                expression = FilterExpression.Equal(FilterExpression.Property("Customer.Email"),
                    FilterExpression.Constant(vendor.Email));
            }
            else
            {
                throw new MessageException($"Can not find customer because name and email is null");
            }

            var modifier = LoadModifiers.Create();
            modifier.Filters = expression;
            sageVendors.Load(modifier);

            if (sageVendors.Count == 0) return null;

            if (sageVendors.Count > 1)
                throw new MessageException($"Found more that one vendor by name: '{vendor.Name}' or email: '{vendor.Email}'");

            sageCustomer = sageVendors.First();
            localDbApi.StoreVendorrId(vendorKey, sageCustomer.ID);
            return sageCustomer;
        }

        /// <summary>
        /// Find or Create and after that Populate Vendor data
        /// </summary>
        /// <param name="vendor"></param>
        /// <param name="sagePayment"></param>
        /// <returns></returns>
        private EntityReference<Vendor> UsertVendor(Models.Data.Vendor vendor, Sage.Peachtree.API.Payment sagePayment)
        {
            var sageVendor = FindSageVendor(vendor) ?? CompanyContext.Factories.VendorFactory.Create();
            sageVendor.PopulateFromModel(CompanyContext, vendor);
            sageVendor.Save();
            return sageVendor.Key;
        }

        public void UpsertInvoice(Models.Data.SalesInvoice invoice)
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

                sagePayment.VendorReference = UsertVendor(payment.Vendor, sagePayment);

                sagePayment.PopulateFromModel(CompanyContext, payment);
            }
        }
    }
}
