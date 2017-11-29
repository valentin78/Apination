using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Newtonsoft.Json;
using Sage50Connector.Core;
using Sage50Connector.Models;
using Sage50Connector.Processing.Actions.SageActions;

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
            return httpUtility.Get("api/actions");
        }

        public string PatchActions(string jsonBody)
        {
            return httpUtility.Patch("api/actions", jsonBody, "application/json");
        }

        public Config RetrieveConnectorConfig()
        {
            var json = httpUtility.Get("api/config");
            return JsonConvert.DeserializeObject<Config>(json);
        }

        /// <summary>
        /// Send handshake to apination from HeartBeat Job
        /// </summary>
        public void Handshake()
        {
            httpUtility.Get("api/heartbeat");
        }
    }
}

