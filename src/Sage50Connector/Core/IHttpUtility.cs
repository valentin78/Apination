using System.Collections.Specialized;

namespace Sage50Connector.Core
{
    public interface IHttpUtility
    {
        string Post(string uri, NameValueCollection parameters);
        string Get(string uri, NameValueCollection parameters);
    }
}
