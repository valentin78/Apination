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
        /// Apination Api Util
        /// </summary>
        protected ApinationRepository ApinationApi => new ApinationRepository(new WebClientHttpUtility());

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

        /// <summary>
        /// Common part of process execution
        /// </summary>
        /// <param name="context"></param>
        protected virtual void Process(IJobExecutionContext context)
        {
        }
    }
}