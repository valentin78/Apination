using System.Collections.Generic;
using Sage50Connector.Models;

namespace Sage50Connector.API
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
                                JobData = new Dictionary<string, object>
                                {
                                    {"p1", "p1Value"},
                                    {"p2", "p2Value"}
                                },
                                AutoStart = true
                            },
                            new SyncProcess
                            {
                                CronSchedule = "0 0/2 * * * ?",
                                ProcessID = "CBD51F9F-4B8D-40A2-B086-1F849894EB96",
                                JobData = new Dictionary<string, object>
                                {
                                    {"p1", "Hello"},
                                    {"p2", "world"}
                                },
                                AutoStart = false
                            }
                        }
                    }
                }
            };
        }
    }
}
