﻿using System.Collections.Specialized;

namespace Sage50Connector.Core
{
    /// <summary>
    /// Interface for Http interoperability
    /// </summary>
    public interface IHttpUtility
    {
        string Post(string uri, NameValueCollection parameters);
        string Post(string uri, string body, string contentType);
        string Get(string uri, NameValueCollection parameters);
        string Get(string uri);
        string Patch(string uri, string body, string contentType);
    }
}
