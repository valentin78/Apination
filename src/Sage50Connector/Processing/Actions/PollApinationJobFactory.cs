using Quartz;
using Quartz.Spi;
using Sage50Connector.API;

namespace Sage50Connector.Processing.Actions
{
    /// <summary>
    /// Creates PollApinationJob using ApinationApi
    /// </summary>
    public class PollApinationJobFactory : IJobFactory
    {
        // ReSharper disable once InconsistentNaming
        private readonly ApinationApi apinationApi;

        public PollApinationJobFactory(ApinationApi apinationApi)
        {
            this.apinationApi = apinationApi;
        }
        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return new PollApinationJob(apinationApi);
        }

        public void ReturnJob(IJob job)
        {
        }
    }
}