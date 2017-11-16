using log4net;
using Sage50Connector.Models;
using Sage50Connector.Temp.Observer.Interface;
using Sage50Connector.Temp.Workflows.FromApination;

namespace Sage50Connector.Temp.Observer
{
    /// <summary>
    /// Fabrik Type for ApinationObserver creation
    /// </summary>
    class ApinationObserverFabrik : IObserverFabrik
    {
        public static readonly ILog Log = LogManager.GetLogger(typeof(ApinationObserverFabrik));

        public IObserver Create(Config config)
        {
            Log.Info("Create CronObserver for ApinationObserver ...");

            var apinationObserver = new CronObserver(config.ApinationCronSchedule);
            
            apinationObserver.AddChecker(new ApinationChecker(config));
            
            apinationObserver.Subscribe(new CreateCustomerToSage50());
            apinationObserver.Subscribe(new CreateInvoiceToSage50());

            return apinationObserver;
        }
    }
}