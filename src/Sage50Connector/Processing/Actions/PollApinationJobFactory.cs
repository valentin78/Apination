using Quartz;
using Quartz.Spi;
using Sage50Connector.API;
using Sage50Connector.Core;

namespace Sage50Connector.Processing.Actions
{
    /// <summary>
    /// Creates PollApinationJob using ApinationApi
    /// </summary>
    public class PollApinationJobFactory : IJobFactory
    {
        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return new PollApinationJob(new ApinationApi(new WebClientHttpUtility()));
        }

        public void ReturnJob(IJob job)
        {
        }
    }
}