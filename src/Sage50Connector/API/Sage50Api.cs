using System;
using System.Linq;
using log4net;
using Sage.Peachtree.API;
using Sage50Connector.Core;
using SalesInvoice = Sage50Connector.Models.Payloads.SalesInvoice;

namespace Sage50Connector.API
{
    /// <summary>
    /// Sage50 SDK wrapper
    /// </summary>
    public class Sage50Api : IDisposable
    {
        public static readonly ILog Log = LogManager.GetLogger(typeof(Sage50Api));

        // ReSharper disable once InconsistentNaming
        private PeachtreeSession ApiSession;
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

        public SalesInvoiceList SalesInvoicesList()
        {
            if (CompanyContext == null) throw new ArgumentException("Company must be open before");
            return CompanyContext.Factories.SalesInvoiceFactory.List();
        }

        public EntityReference<Customer> CreateOrUpdateCustomer(Models.Payloads.Customer customer)
        {
            var customers = CustomersList();

            var sageCustomer = customers.SingleOrDefault(customer.Id) ?? CompanyContext.Factories.CustomerFactory.Create();

            sageCustomer.PopulateFromModel(CompanyContext, customer);
            sageCustomer.Save();

            return sageCustomer.Key;
        }

        public void CreateInvoice(SalesInvoice invoice)
        {
            var sageInvoice = FindInvoice(invoice.ReferenceNumber, invoice.Customer.Id);
            if (sageInvoice != null)
                throw new MessageException($"Found invoice with ReferenceNumber: '{invoice.ReferenceNumber}' and CustomerId: '{invoice.Customer.Id}'. Transaction aborted.");

            // if no exist invoice, we can create new
            sageInvoice = CompanyContext.Factories.SalesInvoiceFactory.Create();

            sageInvoice.CustomerReference = CreateOrUpdateCustomer(invoice.Customer);

            sageInvoice.PopulateFromModel(CompanyContext, invoice);
            sageInvoice.Save();
        }

        public void UpdateInvoice(SalesInvoice invoice)
        {
            var sageInvoice = FindInvoice(invoice.ReferenceNumber, invoice.Customer.Id);
            if (sageInvoice == null)
                throw new MessageException($"Not found invoice with ReferenceNumber: '{invoice.ReferenceNumber}' and CustomerId: '{invoice.Customer.Id}'. Transaction aborted.");

            sageInvoice.CustomerReference = CreateOrUpdateCustomer(invoice.Customer);

            sageInvoice.PopulateFromModel(CompanyContext, invoice);
            sageInvoice.Save();
        }

        /// <summary>
        /// find invoice by reference number and customer id
        /// </summary>
        private Sage.Peachtree.API.SalesInvoice FindInvoice(string referenceNumber, string customerId)
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
    }
}
