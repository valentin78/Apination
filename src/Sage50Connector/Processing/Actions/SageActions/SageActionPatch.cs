using Newtonsoft.Json;

namespace Sage50Connector.Processing.Actions.SageActions
{
    public class SageActionPatch
    {
        [JsonProperty(propertyName: "id")]
        public string Id { get; set; }

        [JsonProperty(propertyName: "processed")]
        public bool Processed { get; set; }
    }
}
