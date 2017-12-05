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

        private readonly Config config;

        public ApinationApi(IHttpUtility httpUtility, Config config)
        {
            this.httpUtility = httpUtility;
            this.config = config;
        }

        /// <summary>
        ///  Gets actions JSON data from apination endpoint
        /// </summary>
        /// <returns></returns>
        public string GetActionsJson()
        {
            return httpUtility.Get(config.ApinationActionEndpointUrl);
        }

        public string ReportProcessingStatus (string jsonBody)
        {
            return httpUtility.Patch(config.ApinationActionEndpointUrl, jsonBody, "application/json");
        }

        public Config GetConnectorConfig()
        {
            var json = httpUtility.Get(ApplicationConfig.ConfigRelativeUrl.Replace(":clientId", ApplicationConfig.ClientId));
            return JsonConvert.DeserializeObject<Config>(json);
        }

        /// <summary>
        /// Send handshake to apination from HeartBeat Job
        /// </summary>
        public void Handshake()
        {
            httpUtility.Post(config.ApinationHeartbeatEndpointUrl, parameters: null);
        }
    }
}

