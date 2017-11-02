using System.Collections.Specialized;
using Newtonsoft.Json;
using Sage50Connector.Core;
using Sage50Connector.Models;

namespace Sage50Connector.Repositories
{
    public class ApinationRepository
    {
        private readonly IHttpUtility _httpUtility;

        public ApinationRepository(IHttpUtility httpUtility)
        {
            _httpUtility = httpUtility;
        }

        public Config RetrieveConnectorConfig()
        {
            var json = _httpUtility.Get("api/config", new NameValueCollection
            {
                {"name", "Chase Ridge Holdings"}
            });

            return JsonConvert.DeserializeObject<Config>(json);
        }

        public void HeartBeat()
        {
            var data = _httpUtility.Post("api/heartbeat", new NameValueCollection
            {
                {"value", "123"}
            });
        }
    }
}

