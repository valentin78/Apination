using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Sage50Connector.Models
{
    /// <summary>
    /// Connector config
    /// </summary>
    public class Config
    {
        /// <summary>
        /// List of companies to be processed in Sage50
        /// </summary>
        public Company[] CompaniesList { get; set; }

        /// <summary>
        /// Default Cron polling period for triggers/actions if not specified in SyncProcess 
        /// https://www.quartz-scheduler.net/documentation/quartz-3.x/tutorial/crontrigger.html
        /// </summary>
        public string DefaultCronSchedule { get; set; }

        /// <summary>
        /// Cron period for HearBeat process
        /// https://www.quartz-scheduler.net/documentation/quartz-3.x/tutorial/crontrigger.html
        /// </summary>
        public string HeartBeatCronSchedule { get; set; }
    }

    public class Company
    {
        /// <summary>
        /// company name
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// processes list
        /// </summary>
        public Process[] Processes { get; set; }
    }

    /// <summary>
    /// Action|Trigger process
    /// </summary>
    public class Process
    {
        /// <summary>
        /// Identifier of process (may be any string to identify Action|Trigger, for example Action.CreateCustomer)
        /// </summary>
        public string ProcessId { get; set; }

        /// <summary>
        /// Cron polling period for SyncProcess
        /// https://www.quartz-scheduler.net/documentation/quartz-3.x/tutorial/crontrigger.html
        /// </summary>
        public string CronSchedule { get; set; }

        /// <summary>
        /// Autostart process when Service starts independent of Cron polling period
        /// </summary>
        public bool AutoStart { get; set; }

        /// <summary>
        /// Parameters set for job parametrization (is not used right now, provided for further enhancement/customization)
        /// </summary>
        public IDictionary<string, object> ProcessParams { get; set; }

        /// <summary>
        /// Used for log purposes
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            using (StringWriter writer = new StringWriter())
            {
                JsonSerializer.Create().Serialize(writer, this);
                return writer.ToString();
            }
        }
    }
}
