// ReSharper disable InconsistentNaming
using System;
using Newtonsoft.Json;

namespace Sage50Connector.Processing.Actions.SageActions
{
    public class SageAction
    {
        [JsonProperty(propertyName: "processingStatus")]
        public ProcessingStatus ProcessingStatus { get; set; }

        [JsonProperty(propertyName: "_id")]
        public string id { get; set; }

        public string type { get; set; }
        public string mainLogId { get; set; }
        public string source { get; set; }
        public int userId { get; set; }
        public int workflowId { get; set; }
        public string triggerId { get; set; }

        public static Type GetActionClassType(string actionType)
        {
            var typeName = typeof(SageAction).FullName;
            var lastPointPosition = typeName.LastIndexOf('.');
            return Type.GetType(typeName.Insert(lastPointPosition + 1, actionType));
        }

    }
}