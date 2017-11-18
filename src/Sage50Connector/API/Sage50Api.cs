using System;
using System.Diagnostics;
using System.Linq;
using Sage.Peachtree.API;
using Sage50Connector.Core;
using Customer = Sage50Connector.Models.Payloads.Customer;

namespace Sage50Connector.API
{
    /// <summary>
    /// Sage50 SDK wrapper
    /// </summary>
    public class Sage50Api : IDisposable
    {
        // ReSharper disable once InconsistentNaming
        private PeachtreeSession ApiSession;
        // ReSharper disable once InconsistentNaming
        private Company CompanyContext;

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

        public void CreateOrUpdateCustomer(Customer customer)
        {
            var customers = CustomersList();
            customers.Load();
            
            var sageCustomer = customers.SingleOrDefault(c => c.ID == customer.Id) ?? CompanyContext.Factories.CustomerFactory.Create();

            sageCustomer.ID = customer.Id;
            sageCustomer.Name = customer.Name;
            sageCustomer.IsInactive = false;
            sageCustomer.AccountNumber = "";

            //Debugger.Launch();

            // set customer bill to contact properties
            sageCustomer.BillToContact.FirstName = customer.BillToContact.FirstName;
            sageCustomer.BillToContact.MiddleInitial = customer.BillToContact.MiddleInitial;
            sageCustomer.BillToContact.LastName = customer.BillToContact.LastName;
            sageCustomer.BillToContact.CompanyName = customer.BillToContact.CompanyName;
            sageCustomer.BillToContact.Address.Address1 = customer.BillToContact.Address.Address1;
            sageCustomer.BillToContact.Address.Address2 = customer.BillToContact.Address.Address2;
            sageCustomer.BillToContact.Address.City = customer.BillToContact.Address.City;
            sageCustomer.BillToContact.Address.State = customer.BillToContact.Address.State;
            sageCustomer.BillToContact.Address.Zip = customer.BillToContact.Address.Zip;
            sageCustomer.BillToContact.Address.Country = customer.BillToContact.Address.Country;

            sageCustomer.BillToContact.Gender = customer.BillToContact.Gender;

            sageCustomer.Save();

            // TODO: ...
        }
    }
}
