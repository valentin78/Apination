using log4net;
using Sage50Connector.Models;
using Sage50Connector.Temp.Observer;
using Sage50Connector.Temp.Workflows.FromApination;

namespace Sage50Connector.Temp.Workflows.FromSage50
{
    class Sage50CustomerChecker : IChecker
    {
        public static readonly ILog Log = LogManager.GetLogger(typeof(ApinationChecker));

        private readonly Config _config;

        public Sage50CustomerChecker(Config config)
        {
            _config = config;
        }
        public void Check(IObserver o)
        {
            Log.InfoFormat("Checking from Sage50 Customers List changes '{0}' ...", o.Identity);

            var companiesList = _config.CompaniesList;
            // foreach companies check customers updates

            // for customers 
            o.TriggerOnDataEvent(new EventData
            {
                Type = "UpdateCustomers",
                Payload = new object() // customersList, companyId
            });
        }
    }
}