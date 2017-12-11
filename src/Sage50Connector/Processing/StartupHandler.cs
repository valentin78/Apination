using System;
using System.Linq;
using log4net;
using Sage50Connector.API;
using Sage50Connector.Core;

namespace Sage50Connector.Processing
{
    class StartupHandler
    {
        public static readonly ILog Log = LogManager.GetLogger(typeof(StartupHandler));

        public static void OnStartup()
        {
            Log.Info("Startup handler started");

            var companiesList = ApplicationConfig.CompaniesList;
            Log.InfoFormat("Retrieved companies list: '{0}'", string.Join(" ; ", companiesList.Select(c => c.Name)));

            var api = new Sage50Api();
            foreach (var company in companiesList)
            {
                try
                {
                    api.OpenCompany(company.Name);
                    api.CloseCurrentCompany();
                    Log.Info($"Company '{company.Name}' opened success");

                }
                catch (Exception ex)
                {
                    Log.Error($"Can not open company: '{company.Name}'; Error message: {ex.Message}");
                }
            }

            Log.Info("Startup handler finished");
        }
    }
}
