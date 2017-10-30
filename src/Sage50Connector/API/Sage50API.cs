using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sage.Peachtree.API;

namespace Sage50Connector.API
{
    class Sage50API : IDisposable
    {
        private PeachtreeSession apiSession;
        private Company companyContext;

        public Sage50API()
        {
            OpenSession();
        }

        void OpenSession()
        {
            apiSession = new PeachtreeSession();
            apiSession.Begin(string.Empty);
        }

        public CompanyIdentifier OpenCompany(string companyName = "Chase Ridge Holdings")
        {
            if (companyContext != null) throw new ArgumentException("Company already opened");

            if (apiSession == null) throw new ArgumentException("Session not opened");
            var companyIdList = apiSession.CompanyList(apiSession.Configuration.ServerName);
            var companyId = companyIdList.SingleOrDefault(c => c.CompanyName == companyName);
            if (companyId == null) throw new ArgumentException("Can't find company by name: " + companyName);

            // Ask the Sage 50 application if this application has
            // been granted access to the company.
            var AuthResult = apiSession.VerifyAccess(companyId);

            // if the app has never asked for authorization before, We need to ask now
            if (AuthResult == AuthorizationResult.NoCredentials)
            {
                AuthResult = apiSession.RequestAccess(companyId);
            }

            // handle the authorization result
            switch (AuthResult)
            {
                case AuthorizationResult.Granted:
                    // open the company
                    companyContext = apiSession.Open(companyId);
                    return companyContext.CompanyIdentifier;
                default:
                    throw new ArgumentException("Can't open company: " + companyName);
            }
        }

        public void Dispose()
        {
            if (apiSession == null) return;
            if (companyContext != null) apiSession.Close(companyContext);
            
            apiSession.End();
            apiSession.Dispose();
        }
    }
}
