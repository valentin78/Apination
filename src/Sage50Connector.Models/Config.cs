﻿using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Sage50Connector.Models
{
    /// <summary>
    /// connector config
    /// </summary>
    public class Config
    {
        /// <summary>
        /// companies list
        /// </summary>
        public Company[] CompaniesList { get; set; }

        /// <summary>
        /// default cron period if not specified 
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
        public SyncProcess[] Processes { get; set; }
    }

    public class SyncProcess
    {
        /// <summary>
        /// equals Guid attribute value for identity Process type to run
        /// </summary>
        public string ProcessID { get; set; }

        /// <summary>
        /// cron period where process start
        /// https://www.quartz-scheduler.net/documentation/quartz-3.x/tutorial/crontrigger.html
        /// </summary>
        public string CronSchedule { get; set; }

        /// <summary>
        /// autostart process with service started 
        /// </summary>
        public bool AutoStart { get; set; }

        /// <summary>
        /// parameters set for job parametrization
        /// </summary>
        public IDictionary<string, object> JobData { get; set; }

        /// <summary>
        /// used for log purposes
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