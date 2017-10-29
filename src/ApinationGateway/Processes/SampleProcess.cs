using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ApinationGateway.Core;
using log4net;
using log4net.Config;
using Quartz;

namespace ApinationGateway.Processes
{
    [Guid("CBD51F9F-4B8D-40A2-B086-1F849894EB96")]
    class SampleProcess: IJob, IProcess
    {
        #region Logger

        public static readonly ILog Log = LogManager.GetLogger(typeof(SampleProcess));

        #endregion

        public void Execute(IJobExecutionContext context)
        {
            Log.Info("-> SampleProcess started ...");
        }
    }
}
