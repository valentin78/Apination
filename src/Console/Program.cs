using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Sage.Peachtree.API;

namespace Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var api = new API();
            var companies = api.CompaniesList();
            //var companyId = companies.SingleOrDefault(c => c.CompanyName == "Chase Ridge Holdings");
            api.OpenCompany(companies[0]);
            var list = api.CustomersList();

            System.Console.WriteLine("Count: {0}", list.Count);

            list.ListChanged += (sender, eventArgs) =>
            {
                Debugger.Launch();
            };

        }
    }

    class API : IDisposable
    {
        private PeachtreeSession m_Session = null;
        /// <summary>
        /// the active Sage 50 Company
        /// </summary>
        private Company m_Company = null;

        public void Dispose()
        {
            CloseCurrentCompany();

            if (m_Session != null)
            {
                m_Session.Dispose();
                m_Session = null;
            }
        }

        protected PeachtreeSession CurrentSession
        {
            get
            {
                // if the session has not been initialized
                if (m_Session == null || !m_Session.SessionActive)
                {
                    // dispose of the inactive session
                    if (m_Session != null)
                    {
                        m_Session.Dispose();
                    }

                    // create a new session instance
                    m_Session = new PeachtreeSession();

                    // start the session.  
                    // with no application ID, you can only open Sample companies
                    m_Session.Begin("");
                }
                // return the current session
                return m_Session;
            }
        }

        private void CloseCurrentCompany()
        {
            if (m_Company != null)
            {
                m_Company.Close();
                m_Company = null;
            }
        }

        public void OpenCompany(CompanyIdentifier CompanyID)
        {
            CloseCurrentCompany();

            if (CompanyID == null) return;

            // Ask the Sage 50 application if this application has
            // been granted access to the company.
            var AuthResult = CurrentSession.VerifyAccess(CompanyID);

            // if the app has never asked for authorization before, We need to ask now
            if (AuthResult == AuthorizationResult.NoCredentials)
            {
                AuthResult = CurrentSession.RequestAccess(CompanyID);
            }

            // handle the authorization result
            switch (AuthResult)
            {
                case AuthorizationResult.CompanyLocked:
                case AuthorizationResult.LoginRestricted:
                case AuthorizationResult.Denied:
                case AuthorizationResult.CorruptedOrTampered:
                case AuthorizationResult.Pending:
                    // show the error messages for these situations
                    System.Console.WriteLine($"Error OpenCompany_{AuthResult.ToString()}");
                    break;

                case AuthorizationResult.Granted:
                    // open the company
                    m_Company = CurrentSession.Open(CompanyID);
                    break;
                default:
                    Debug.Fail(AuthResult.ToString());
                    break;
            }

        }

        public CompanyIdentifierList CompaniesList()
        {
            return CurrentSession.CompanyList();
        }

        public CustomerList CustomersList()
        {
            return m_Company.Factories.CustomerFactory.List();
        }
    }
}
