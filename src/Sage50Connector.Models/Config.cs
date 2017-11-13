using System;
using System.IO;
using Newtonsoft.Json;
using Sage50Connector.Models.BindingTypes;

namespace Sage50Connector.Models
{
    /// <summary>
    /// Connector config
    /// </summary>
    [Serializable]
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
        /// Apination ReST endpoint URL for Actions
        /// </summary>
        public string ApinationActionEndpointUrl { get; set; }

        /// <summary>
        /// Triggers Config
        /// </summary>
        public Sage50TriggersConfig[] TriggersConfig { get; set; }

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
    /// Sage50 Trigger Config
    /// </summary>
    public class Sage50TriggersConfig
    {
        /// <summary>
        /// Trigger binding Type
        /// </summary>
        public Sage50EventBindingTypes TriggerBindingType { get; set; }

        /// <summary>
        /// Apination Endpoint URL for trigger type
        /// </summary>
        public string ApinationEndpointUrl { get; set; }
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
