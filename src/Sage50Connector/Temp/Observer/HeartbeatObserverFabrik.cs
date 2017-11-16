using log4net;
using Sage50Connector.Models;
using Sage50Connector.Temp.Workflows.FromApination;
using Sage50Connector.Temp.Workflows.HeartBeat;

namespace Sage50Connector.Temp.Observer
{
    /// <summary>
    /// Fabrik Type for HeartbeatObserver creation
    /// </summary>
    class HeartbeatObserverFabrik
    {
        public static readonly ILog Log = LogManager.GetLogger(typeof(HeartbeatObserverFabrik));

        public IObserver Create(Config config)
        {
            Log.Info("Create CronObserver for HeartbeatObserverFabrik ...");

            var observer = new CronObserver(config.HeartBeatCronSchedule);

            observer.AddChecker(new HeartbeatChecker(config));

            return observer;
        }
    }
}