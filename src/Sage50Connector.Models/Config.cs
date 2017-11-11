using System;
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
        /// Cron polling period for Sage50 Observer activation
        /// https://www.quartz-scheduler.net/documentation/quartz-3.x/tutorial/crontrigger.html
        /// </summary>
        public string Sage50CronSchedule { get; set; }

        /// <summary>
        /// Cron polling period for Apination Observer activation
        /// https://www.quartz-scheduler.net/documentation/quartz-3.x/tutorial/crontrigger.html
        /// </summary>
        public string ApinationCronSchedule { get; set; }

        /// <summary>
        /// Cron period for HearBeat process
        /// https://www.quartz-scheduler.net/documentation/quartz-3.x/tutorial/crontrigger.html
        /// </summary>
        public string HeartBeatCronSchedule { get; set; }

        /// <summary>
        /// Serialize object to JSON string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            using (var writer = new StringWriter())
            {
                JsonSerializer.Create().Serialize(writer, this);
                return writer.ToString();
            }
        }
    }

    /// <summary>
    /// Company Data
    /// </summary>
    public class Company
    {
        /// <summary>
        /// company name
        /// </summary>
        public string CompanyName { get; set; }
    }
}
