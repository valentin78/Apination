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
                                ProcessID = "CBD51F9F-4B8D-40A2-B086-1F849894EB96", 
                                AutoStart = true
                            }
                        }
                    }
                }
            };
        }
    }
}
