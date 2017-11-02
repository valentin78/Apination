using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Net;
using System.Text;

namespace Sage50Connector.Core
{
    class HttpUtil
    {
        public static string AuthToken => ConfigurationManager.AppSettings["AuthToken"];
        public static Uri ApinationBaseUri => new Uri(ConfigurationManager.AppSettings["ApinationBaseUrl"]);

        public static string Post(string uri, NameValueCollection parameters)
        {
            var remoteUri = new Uri(ApinationBaseUri, uri);

            WebClient client = new WebClient();
            client.Headers.Add("AuthToken", AuthToken);

            return Encoding.UTF8.GetString(client.UploadValues(remoteUri, parameters));
        }

        public static string Get(string uri, NameValueCollection parameters)
        {
            var remoteUri = new Uri(ApinationBaseUri, uri);

            WebClient client = new WebClient();
            client.Headers.Add("AuthToken", AuthToken);
            foreach (string parameter in parameters.Keys)
            {
                client.QueryString.Add(parameter, parameters[parameter]);
            }

            return client.DownloadString(remoteUri);
        }
    }
}