using System;
using System.Reflection;
using log4net;
using Quartz;
using Sage50Connector.API;

namespace Sage50Connector.Core
{
    public abstract class ProcessBase: IJob
    {
        /// <summary>
        /// ILog instance for logging purpose
        /// </summary>
        public static readonly ILog Log = LogManager.GetLogger(typeof(ProcessBase));

        /// <summary>
        /// Sage50 Api
        /// </summary>
        protected Sage50Api Sage50Api => new Sage50Api();

        /// <summary>
        /// Apination Api Util
        /// </summary>
        protected ApinationApi ApinationApi => new ApinationApi(new WebClientHttpUtility());

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                Process(context);
            }
            catch (Exception ex)
            {
                Log.Error("Job execution failure", ex);
            }
        }

        /// <summary>
        /// Common part of process execution
        /// </summary>
        /// <param name="context"></param>
        protected abstract void Process(IJobExecutionContext context);
    }
}