using System.Collections.Specialized;
using Newtonsoft.Json;
using Sage50Connector.Core;
using Sage50Connector.Models;

namespace Sage50Connector.API
{
    /// <summary>
    /// Apination interface wrapper
    /// </summary>
    public class ApinationApi
    {
        // ReSharper disable once InconsistentNaming
        private readonly IHttpUtility httpUtility;

        public ApinationApi(IHttpUtility httpUtility)
        {
            this.httpUtility = httpUtility;
        }

        /// <summary>
        ///  Gets actions JSON data from apination endpoint
        /// </summary>
        /// <returns></returns>
        public string GetActionsJson()
        {
            return httpUtility.Get("api/actions", new NameValueCollection
            {
                {"name", "Chase Ridge Holdings"}
            });
        }

        public Config RetrieveConnectorConfig()
        {
            var json = httpUtility.Get("api/config", new NameValueCollection
            {
                {"name", "Chase Ridge Holdings"}
            });

            return JsonConvert.DeserializeObject<Config>(json);
        }

        public void HeartBeat()
        {
            httpUtility.Post("api/heartbeat", new NameValueCollection
            {
                {"value", "123"}
            });
        }
    }
}

