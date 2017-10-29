using System.Runtime.InteropServices;
using ApinationGateway.Core;
using Quartz;

namespace ApinationGateway.Processes
{
    [Guid("CBD51F9F-4B8D-40A2-B086-1F849894EB96")]
    class SampleProcess: ProcessBase, IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            Log.Info("-> SampleProcess started ...");
        }
    }
}
