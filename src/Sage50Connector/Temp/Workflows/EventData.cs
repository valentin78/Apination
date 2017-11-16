using System.IO;
using Newtonsoft.Json;

namespace Sage50Connector.Temp.Workflows
{
    class EventData
    {
        public string Type { get; set; }
        public object Payload { get; set; }

        public override string ToString()
        {
            using (var writer = new StringWriter())
            {
                JsonSerializer.Create().Serialize(writer, this);
                return writer.ToString();
            }
        }
    }
}