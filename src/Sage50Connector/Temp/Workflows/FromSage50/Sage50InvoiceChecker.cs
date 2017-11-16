using log4net;
using Sage50Connector.Models;
using Sage50Connector.Temp.Observer;
using Sage50Connector.Temp.Observer.Interface;

namespace Sage50Connector.Temp.Workflows.FromSage50
{
    class Sage50InvoiceChecker : IChecker
    {
        public static readonly ILog Log = LogManager.GetLogger(typeof(Sage50InvoiceChecker));

        private readonly Config _config;

        public Sage50InvoiceChecker(Config config)
        {
            _config = config;
        }
        public void Check(IObserver o)
        {
            Log.InfoFormat("Checking from Sage50 Invoices List changes '{0}' ...", o.Identity);

            var companiesList = _config.CompaniesList;
            // foreach companies check invoices updates

            // for invoices 
            o.TriggerOnDataEvent(new EventData
            {
                Type = "UpdateInvoices",
                Payload = new object() // invoicesList, companyId
            });
        }
    }
}