using System;
using Quartz;
using Quartz.Spi;
using Sage50Connector.API;

namespace Sage50Connector.Processing
{
    /// <summary>
    /// Creates Job using ApinationApi
    /// </summary>
    public class JobFactory : IJobFactory
    {
        // ReSharper disable once InconsistentNaming
        private readonly ApinationApi apinationApi;

        public JobFactory(ApinationApi apinationApi)
        {
            this.apinationApi = apinationApi;
        }
        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return Activator.CreateInstance(bundle.JobDetail.JobType, apinationApi) as IJob;
        }

        public void ReturnJob(IJob job)
        {
        }
    }
}