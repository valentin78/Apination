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
            var _apiSession = new PeachtreeSession();
            try
            {
                _apiSession.Begin("");

                var companyIdList = _apiSession.CompanyList(_apiSession.Configuration.ServerName);
                var companyId = companyIdList.SingleOrDefault(c => c.CompanyName == "Chase Ridge Holdings");

                var authResult = _apiSession.VerifyAccess(companyId);
                
                if (authResult == AuthorizationResult.NoCredentials)
                {
                    authResult = _apiSession.RequestAccess(companyId);
                }

                switch (authResult)
                {
                    case AuthorizationResult.Granted:
                        
                        // open the company
                        var _companyContext = _apiSession.Open(companyId);
                        var list = _companyContext.Factories.CustomerFactory.List();
                        
                        System.Console.WriteLine("Count: {0}", list.Count);
                        
                        list.ListChanged += (sender, eventArgs) =>
                        {
                            Debugger.Launch();
                        };
                        
                        break;

                    default:
                        System.Console.WriteLine("Can't open company. Require permissions.");
                        break;
                }

            }
            finally
            {
                System.Console.WriteLine("Press ENTER ...");

                System.Console.ReadKey();
                _apiSession.Dispose();
            }
        }
    }
}
