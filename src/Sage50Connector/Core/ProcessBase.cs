using System;
using log4net;
using Quartz;
using Sage50Connector.API;

namespace Sage50Connector.Core
{
    public class ProcessBase: IJob
    {
        /// <summary>
        /// ILog instance for logging purpose
        /// </summary>
        public static readonly ILog Log = LogManager.GetLogger(typeof(ProcessBase));

        /// <summary>
        /// Apination API Helper
        /// </summary>
        protected ApinationAPI _apinationApi => new ApinationAPI();

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                Process(context);
            }
            catch (Exception ex)
            {
                Log.Error("Job excetion failure", ex);
            }
        }

        protected virtual void Process(IJobExecutionContext context)
        {
        }
    }
}