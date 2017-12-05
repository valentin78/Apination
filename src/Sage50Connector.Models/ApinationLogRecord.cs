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
        /// ���� "triggerId" �� �������
        /// </summary>
        [JsonProperty(PropertyName = "trigger_id")]
        public string TriggerId { get; set; }

        /// <summary>
        /// ���� "mainLogId" �� �������
        /// </summary>
        [JsonProperty(PropertyName = "uid")]
        public string Uid { get; set; }

        /// <summary>
        /// ������������ JSON � �������� ������, ��� {}
        /// </summary>
        [JsonProperty(PropertyName = "data")]
        public string Data { get; set; }

        /// <summary>
        /// ���� � ������� ISO
        /// </summary>
        [JsonProperty(PropertyName = "date")]
        public DateTime Date { get; set; }
    }
}