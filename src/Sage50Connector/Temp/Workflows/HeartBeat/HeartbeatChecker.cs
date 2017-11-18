using log4net;
using Sage50Connector.Models;
using Sage50Connector.Temp.Observer;

namespace Sage50Connector.Temp.Workflows.HeartBeat
{
    class HeartbeatChecker : IChecker
    {
        public static readonly ILog Log = LogManager.GetLogger(typeof(HeartbeatChecker));


        private readonly Config _config;

        public HeartbeatChecker(Config config)
        {
            _config = config;
        }

        void IChecker.Check(IObserver o)
        {
            Log.InfoFormat("Heartbeat '{0}' ...", o.Identity);
        }
    }
}