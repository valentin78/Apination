using System;
using System.Diagnostics;
using System.Linq;
using Sage.Peachtree.API;
using Sage.Peachtree.API.Collections.Generic;
using Sage50Connector.Core;

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
        private Company CompanyContext { get; set; }

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

        public void CreateOrUpdateCustomer(Models.Payloads.Customer customer)
        {
            var customers = CustomersList();

            var sageCustomer = customers.SingleOrDefault(customer.Id) ?? CompanyContext.Factories.CustomerFactory.Create();

            sageCustomer.PopulateFromModel(CompanyContext, customer);
            sageCustomer.Save();
        }
    }
}
