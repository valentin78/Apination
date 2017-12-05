using System;
using Newtonsoft.Json;

namespace Sage50Connector.Models
{
    public class ApinationLogRecord
    {
        /// <summary>
        /// Error message
        /// </summary>
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        /// <summary>
        /// Success | Fail | Ignored
        /// </summary>
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        /// <summary>
        /// поле "triggerId" из экшенов
        /// </summary>
        [JsonProperty(PropertyName = "trigger_id")]
        public string TriggerId { get; set; }

        /// <summary>
        /// поле "mainLogId" из экшенов
        /// </summary>
        [JsonProperty(PropertyName = "uid")]
        public string Uid { get; set; }

        /// <summary>
        /// произвольный JSON с деталями ошибки, или {}
        /// </summary>
        [JsonProperty(PropertyName = "data")]
        public string Data { get; set; }

        /// <summary>
        /// дата в формате ISO
        /// </summary>
        [JsonProperty(PropertyName = "date")]
        public DateTime Date { get; set; }
    }
}