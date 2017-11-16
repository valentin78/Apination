using log4net;
using Sage50Connector.Models;
using Sage50Connector.Temp.Observer;

namespace Sage50Connector.Temp.Workflows.FromApination
{
    class ApinationChecker : IChecker
    {
        public static readonly ILog Log = LogManager.GetLogger(typeof(ApinationChecker));


        private readonly Config _config;

        public ApinationChecker(Config config)
        {
            _config = config;
        }

        void IChecker.Check(IObserver o)
        {
            Log.InfoFormat("Checking from Apination Service Data changes '{0}' ...", o.Identity);

            var url = _config.ApinationActionEndpointUrl;

            // get data from Apination Endpoint
            // parsing responce 

            // customers first order
            o.TriggerOnDataEvent(new EventData
            {
                Type = "CreateCustomer",
                Payload = new object()
            });

            // invoices second, after customers
            o.TriggerOnDataEvent(new EventData
            {
                Type = "CreateInvoice",
                Payload = new object()
            });
        }

    }
}