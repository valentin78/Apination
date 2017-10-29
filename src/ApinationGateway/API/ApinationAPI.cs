using ApinationGateway.Models;

namespace ApinationGateway.API
{
    class ApinationAPI
    {
        public Config RetrieveGatewayConfig()
        {
            return new Config
            {
                CompaniesList = new[]
                {
                    new Company
                    {
                        CompanyName = "Demo Company",
                        Processes = new [] { 
                            new SyncProcess
                            {
                                CronSchedule = "0 0/1 * * * ?", 
                                ProcessID = null, 
                                AutoStart = true
                            }
                        }
                    }
                }
            };
        }
    }
}
