using System.Runtime.InteropServices;
using Quartz;
using Sage50Connector.Core;

namespace Sage50Connector.Processes
{
    [Guid("CBD51F9F-4B8D-40A2-B086-1F849894EB96")]
    class SampleProcess : ProcessBase, IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            var p1 = context.JobDetail.JobDataMap["p1"];
            var p2 = context.JobDetail.JobDataMap["p2"];
            Log.InfoFormat("-> SampleProcess started ... p1: {0}; p2: {1}", p1, p2);
        }
    }
}
