using System;
using log4net;
using Quartz;
using Sage50Connector.Repositories;

namespace Sage50Connector.Core
{
    public class ProcessBase: IJob
    {
        /// <summary>
        /// ILog instance for logging purpose
        /// </summary>
        public static readonly ILog Log = LogManager.GetLogger(typeof(ProcessBase));

        /// <summary>
        /// Apination Api Helper
        /// </summary>
        protected ApinationRepository _apinationApi => new ApinationRepository();

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                Process(context);
            }
            catch (Exception exc)
            {
                Log.Error("Job excetion failure", exc);
            }
        }

        protected virtual void Process(IJobExecutionContext context)
        {
        }
    }
}