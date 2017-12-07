﻿using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using log4net;

namespace Sage50Connector.Core
{
    /// <summary>
    /// HttpUtility implementation using WebClient and Auth token
    /// </summary>
    public class WebClientHttpUtility: IHttpUtility
    {
        public static readonly ILog Log = LogManager.GetLogger(typeof(WebClientHttpUtility));
        private static WebClient HttpClientFactory() {
            WebClient client = new WebClient();
            client.Headers.Add("Authorization", ApplicationConfig.AuthToken);
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
                var response = client.UploadValues(ToAbsoluteUrl(uri), parameters ?? new NameValueCollection());
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
                Log.Debug(ToAbsoluteUrl(uri));
                return client.DownloadString(ToAbsoluteUrl(uri));
            }
        }

        public string Get(string uri)
        {
            return Get(uri, new NameValueCollection());
        }

        public string Patch(string uri, string body, string contentType)
        {
            using (var client = HttpClientFactory())
            {
                client.Headers["Content-Type"] = contentType;
                client.Headers["Accept"] = contentType;

                Log.DebugFormat("Patch to URL: {0}", ToAbsoluteUrl(uri));
                var result = client.UploadData(ToAbsoluteUrl(uri), "PATCH", Encoding.UTF8.GetBytes(body));
                return Encoding.UTF8.GetString(result);
            }
        }
    }
}