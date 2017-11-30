using System;
using System.IO;
using Newtonsoft.Json;

namespace Sage50Connector.Models
{
    /// <summary>
    /// Connector config
    /// </summary>
    [Serializable]
    public class Config
    {
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
        /// Apination ReST endpoint URL for Heartbeat
        /// </summary>
        public string ApinationHeartbeatEndpointUrl { get; set; }

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
}
