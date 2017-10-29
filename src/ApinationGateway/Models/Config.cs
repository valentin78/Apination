using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace ApinationGateway.Models
{
    class Config
    {
        public Company[] CompaniesList { get; set; }
    }

    internal class Company
    {
        public string CompanyName { get; set; }

        public SyncProcess[] Processes { get; set; }
    }

    internal class SyncProcess
    {
        public string ProcessID { get; set; }

        /// <summary>
        /// Кроновский период, когда будет запускаться процесс
        /// doc. https://www.quartz-scheduler.net/documentation/quartz-3.x/tutorial/crontrigger.html
        /// </summary>
        public string CronSchedule { get; set; }

        /// <summary>
        /// Автозапуск процесса со стартом сервиса (не дожидаясь графика запуска)
        /// </summary>
        public bool AutoStart { get; set; }

        public override string ToString()
        {
            using (StringWriter writer = new StringWriter())
            {
                JsonSerializer.Create().Serialize(writer, this);
                return writer.ToString();
            }
        }

        public IDictionary<string, object> JobData { get; set; }
    }
}
