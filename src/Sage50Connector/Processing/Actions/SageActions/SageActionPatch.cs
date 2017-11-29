using System;
using Newtonsoft.Json;

namespace Sage50Connector.Processing.Actions.SageActions
{
    public enum Status
    {
        SUCCESS = 1, 
        FAIL = 2,
        NOT_PROCESSED = 3
    }
    public class ProcessingStatus
    {
        public ProcessingStatus()
        {
            Status = Status.NOT_PROCESSED;
            ProcessedAt = DateTime.Now;
        }
        [JsonProperty(propertyName: "status")]
        public Status Status { get; set; }

        [JsonProperty(propertyName: "error")]
        public string Error { get; set; }

        [JsonProperty(propertyName: "processedAt")]
        public DateTime ProcessedAt { get; set; }
    }
}
