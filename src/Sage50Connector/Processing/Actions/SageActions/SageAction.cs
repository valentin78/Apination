// ReSharper disable InconsistentNaming
using System;
using Newtonsoft.Json;

namespace Sage50Connector.Processing.Actions.SageActions
{
    public class SageAction
    {
        [JsonProperty(propertyName: "processingStatus")]
        public ProcessingStatus ProcessingStatus { get; set; }

        public string type { get; set; }
        public int mainLogId { get; set; }
        public string source { get; set; }
        public int userId { get; set; }
        public int workflowId { get; set; }
        public int triggerId { get; set; }

        public static Type GetActionClassType(string actionType)
        {
            return Type.GetType($"Sage50Connector.Processing.Actions.SageActions.{actionType}SageAction");
        }

    }
}