using log4net;
using Sage50Connector.Models;

namespace Sage50Connector.Temp.Workflows.FromSage50
{
    class UpdateInvoicesToApination : ISaver
    {
        public static readonly ILog Log = LogManager.GetLogger(typeof(UpdateInvoicesToApination));

        private Config _config;

        public UpdateInvoicesToApination(Config config)
        {
            _config = config;
        }
        public string PayloadTypeFiler => "UpdateInvoices";

        public void Save(EventData data)
        {
            Log.InfoFormat("POST Invoices to Apination: {0}", data);

            // url for UpdateCustomers
            //var url = _config.TriggersConfig[PayloadTypeFiler].ApinationEndpointUrl;
            // send invoices to Apination URL
        }
    }
}
