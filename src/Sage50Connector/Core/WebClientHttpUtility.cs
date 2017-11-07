using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;

namespace Sage50Connector.Core
{
    /// <summary>
    /// HttpUtility implementation using WebClient and Auth token
    /// </summary>
    public class WebClientHttpUtility: IHttpUtility
    {
        private static WebClient HttpClientFactory() {
            WebClient client = new WebClient();
            client.Headers.Add("AuthToken", ApplicationConfig.AuthToken);
            return client;
        }

        private static Uri ToAbsoluteUrl(string uri)
        {
            return new Uri(ApplicationConfig.ApinationBaseUri, uri);
        }

        /// <summary>
        /// Uploads the specified name/value collection to the resource identified by the specified URI via POST Request
        /// content-type: application/x-www-формы-urlencoded
        /// </summary>
        /// <param name="uri">resource URI</param>
        /// <param name="parameters">name/value collection</param>
        /// <returns></returns>
        public string Post(string uri, NameValueCollection parameters)
        {
            using (var client = HttpClientFactory())
            {
                var response = client.UploadValues(ToAbsoluteUrl(uri), parameters);
                return Encoding.UTF8.GetString(response);
            }
        }

        /// <summary>
        /// Downloads the requested resource as a String via GET request with query parameters specified in name/value collection
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="parameters">name/value collection</param>
        /// <returns></returns>
        public string Get(string uri, NameValueCollection parameters)
        {
            using (var client = HttpClientFactory())
            {
                foreach (string parameter in parameters.Keys)
                {
                    client.QueryString.Add(parameter, parameters[parameter]);
                }

                return client.DownloadString(ToAbsoluteUrl(uri));
            }
        }
    }
}