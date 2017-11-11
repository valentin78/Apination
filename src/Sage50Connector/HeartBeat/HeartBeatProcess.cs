using System;
using log4net;
using Quartz;
using Sage50Connector.API;
using Sage50Connector.Core;
using Sage50Connector.Processing;

namespace Sage50Connector.HeartBeat
{
    class HeartBeatProcess : IJob
    {
        /// <summary>
        /// ILog instance for logging purpose
        /// </summary>
        public static readonly ILog Log = LogManager.GetLogger(typeof(Sage50Observer));

        /// <summary>
        /// Apination Api Util
        /// </summary>
        protected ApinationApi ApinationApi => new ApinationApi(new WebClientHttpUtility());

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                Log.Info("-> HeartBeatProcess started");
                ApinationApi.HeartBeat();
            }
            catch (Exception ex)
            {
                Log.Error("Job execution failure", ex);
            }
        }
    }
}
