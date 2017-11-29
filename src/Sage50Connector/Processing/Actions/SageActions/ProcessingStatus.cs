using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

// ReSharper disable InconsistentNaming

namespace Sage50Connector.Processing.Actions.SageActions
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Status
    {
        SUCCESS, 
        FAIL,
        NOT_PROCESSED
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
