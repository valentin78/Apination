﻿namespace ApinationGateway.Models
{
    class Config
    {
        public Company[] CompaniesList { get; set; }
    }

    internal class Company
    {
        public string CompanyName { get; set; }

        public SyncProcess Process { get; set; }
    }

    internal class SyncProcess
    {
        public string ProcessID { get; set; }

        /// <summary>
        /// Кроновский период, когда будет запускаться процесс
        /// doc. https://www.quartz-scheduler.net/documentation/quartz-3.x/tutorial/crontrigger.html
        /// </summary>
        public string CronSchedule { get; set; }
    }
}