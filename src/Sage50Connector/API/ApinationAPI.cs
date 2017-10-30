using Sage50Connector.Models;
using System.Collections.Specialized;
using Newtonsoft.Json;
using Sage50Connector.Core;

namespace Sage50Connector.API
{
    public class ApinationAPI
    {
        public Config RetrieveConnectorConfig()
        {
            var json = HttpHelper.Get("api/config", new NameValueCollection
            {
                {"name", "Chase Ridge Holdings"}
            });

            return JsonConvert.DeserializeObject<Config>(json);
        }

        public void HearBest()
        {
            var data = HttpHelper.Post("api/heartbeat", new NameValueCollection
            {
                {"value", "123"}
            });
        }
    }
}

