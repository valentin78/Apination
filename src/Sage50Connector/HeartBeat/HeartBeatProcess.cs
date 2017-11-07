using Quartz;
using Sage50Connector.Core;

namespace Sage50Connector.HeartBeat
{
    class HeartBeatProcess : ProcessBase
    {
        protected override void Process(IJobExecutionContext context)
        {
            base.Process(context);

            Log.Info("-> HeartBeatProcess started");
            ApinationApi.HeartBeat();
        }
    }
}
