using log4net;

namespace ApinationGateway.Core
{
    public class ProcessBase
    {
        /// <summary>
        /// ILog instance for logging purpose
        /// </summary>
        public static readonly ILog Log = LogManager.GetLogger(typeof(ProcessBase));
    }
}