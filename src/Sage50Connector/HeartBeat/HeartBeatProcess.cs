using Quartz;
using Sage50Connector.Core;

namespace Sage50Connector.HeartBeat
{
    class HeartBeatProcess : ProcessBase, IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            Log.Info("-> HeartBeatProcess ...");
        }
    }
}
