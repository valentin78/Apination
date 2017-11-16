using log4net;
using Sage50Connector.Models;
using Sage50Connector.Temp.Observer.Interface;
using Sage50Connector.Temp.Workflows.FromSage50;

namespace Sage50Connector.Temp.Observer
{
    /// <summary>
    /// Fabrik Type for Sage50Observer creation
    /// </summary>
    class Sage50ObserverFabrik 
    {
        public static readonly ILog Log = LogManager.GetLogger(typeof(Sage50ObserverFabrik));

        public IObserver Create(Config config)
        {
            Log.Info("Create CronObserver for Sage50ObserverFabrik ...");

            var sage50Observer = new CronObserver(config.Sage50CronSchedule);
            
            sage50Observer.AddChecker(new Sage50CustomerChecker(config));
            sage50Observer.AddChecker(new Sage50InvoiceChecker(config));
            
            sage50Observer.Subscribe(new UpdateCustomersToApination(config));
            sage50Observer.Subscribe(new UpdateInvoicesToApination(config));

            return sage50Observer;
        }
    }
}