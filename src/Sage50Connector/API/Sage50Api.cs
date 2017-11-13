using System;
using System.Linq;
using Sage.Peachtree.API;
using Sage50Connector.Core;

namespace Sage50Connector.API
{
    /// <summary>
    /// Sage50 SDK wrapper
    /// </summary>
    public class Sage50Api : IDisposable
    {
        private PeachtreeSession _apiSession;
        private Company _companyContext;

        protected PeachtreeSession CurrentSession
        {
            get
            {
                // if the session has not been initialized
                if (_apiSession != null && _apiSession.SessionActive) return _apiSession;

                // dispose of the inactive session
                _apiSession?.Dispose();

                // create a new session instance
                _apiSession = new PeachtreeSession();

                // start the session.  
                // with no application ID, you can only open Sample companies
                _apiSession.Begin(ApplicationConfig.Sage50ApplicationID);
                // return the current session
                return _apiSession;
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
                    _companyContext = CurrentSession.Open(companyId);
                    break;
                default:
                    throw new ArgumentException($"Can not open company {companyName}. Authorization Result: {authResult}");
            }
        }

        public void Dispose()
        {
            CloseCurrentCompany();
            _apiSession?.Dispose();
        }

        private void CloseCurrentCompany()
        {
            if (_companyContext == null) return;

            _companyContext.Close();
            _companyContext = null;
        }

        public CompanyIdentifierList CompaniesList()
        {
            return CurrentSession.CompanyList(CurrentSession.Configuration.ServerName);
        }

        public CompanyIdentifier FindCompany(string companyName)
        {
            var result = CompaniesList().SingleOrDefault(c => c.CompanyName == companyName);
            if (result == null) throw new ArgumentException($"Can not find company by name: {companyName}");
            return result;
        }

        public CustomerList CustomersList()
        {
            if (_companyContext == null) throw new ArgumentException("Company must be open before");
            return _companyContext.Factories.CustomerFactory.List();
        }
    }
}
