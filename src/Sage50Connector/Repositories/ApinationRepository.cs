using System.Collections.Specialized;
using Newtonsoft.Json;
using Sage50Connector.Core;
using Sage50Connector.Models;

namespace Sage50Connector.Repositories
{
    public class ApinationRepository
    {
        public Config RetrieveConnectorConfig()
        {
            var json = HttpHelper.Get("api/config", new NameValueCollection
            {
                {"name", "Chase Ridge Holdings"}
            });

            return JsonConvert.DeserializeObject<Config>(json);
        }

        public void HeartBeat()
        {
            var data = HttpHelper.Post("api/heartbeat", new NameValueCollection
            {
                {"value", "123"}
            });
        }
    }
}

