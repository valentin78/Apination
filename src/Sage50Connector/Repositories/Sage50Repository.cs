using System;
using System.Linq;
using Sage.Peachtree.API;
using Sage50Connector.Core;

namespace Sage50Connector.Repositories
{
    class Sage50Repository : IDisposable
    {
        private PeachtreeSession _apiSession;
        private Company _companyContext;

        public Sage50Repository()
        {
            OpenSession();
        }

        void OpenSession()
        {
            _apiSession = new PeachtreeSession();
            _apiSession.Begin(ApplicationConfig.Sage50ApplicationID);
        }

        public CompanyIdentifier OpenCompany(string companyName = "Chase Ridge Holdings")
        {
            if (_companyContext != null) throw new ArgumentException("Company already opened");

            if (_apiSession == null) throw new ArgumentException("Session not opened");
            var companyIdList = _apiSession.CompanyList(_apiSession.Configuration.ServerName);
            var companyId = companyIdList.SingleOrDefault(c => c.CompanyName == companyName);
            if (companyId == null) throw new ArgumentException("Can't find company by name: " + companyName);

            // Ask the Sage 50 application if this application has
            // been granted access to the company.
            var authResult = _apiSession.VerifyAccess(companyId);

            // if the app has never asked for authorization before, We need to ask now
            if (authResult == AuthorizationResult.NoCredentials)
            {
                authResult = _apiSession.RequestAccess(companyId);
            }

            // handle the authorization result
            switch (authResult)
            {
                case AuthorizationResult.Granted:
                    // open the company
                    _companyContext = _apiSession.Open(companyId);
                    return _companyContext.CompanyIdentifier;
                default:
                    throw new ArgumentException("Can't open company: " + companyName);
            }
        }

        public void Dispose()
        {
            if (_apiSession == null) return;
            if (_companyContext != null) _apiSession.Close(_companyContext);
            
            _apiSession.End();
            _apiSession.Dispose();
        }
    }
}
