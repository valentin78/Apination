using log4net;
using Sage50Connector.Models;

namespace Sage50Connector.Temp.Workflows.FromSage50
{
    class UpdateCustomersToApination : ISaver
    {
        public static readonly ILog Log = LogManager.GetLogger(typeof(UpdateCustomersToApination));

        private Config _config;

        public UpdateCustomersToApination(Config config)
        {
            _config = config;
        }
        public string PayloadTypeFiler => "UpdateCustomers";

        public void Save(EventData data)
        {
            Log.InfoFormat("POST Customers to Apination: {0}", data);

            // url for UpdateCustomers
            //var url = _config.TriggersConfig[PayloadTypeFiler].ApinationEndpointUrl;
            // send customers to Apination URL
        }
    }
}