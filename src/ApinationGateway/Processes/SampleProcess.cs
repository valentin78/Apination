using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Config;
using Quartz;

namespace ApinationGateway.Processes
{
    class SampleProcess: IJob
    {
        #region Logger

        public static readonly ILog Log = LogManager.GetLogger(typeof(SampleProcess));

        #endregion

        public void Execute(IJobExecutionContext context)
        {
            Log.Info("*** SampleProcess started ...");
        }
    }
}
