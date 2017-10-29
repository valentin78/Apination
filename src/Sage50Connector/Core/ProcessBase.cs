using log4net;
using Sage50Connector.API;

namespace Sage50Connector.Core
{
    public class ProcessBase
    {
        /// <summary>
        /// ILog instance for logging purpose
        /// </summary>
        public static readonly ILog Log = LogManager.GetLogger(typeof(ProcessBase));

        /// <summary>
        /// Apination API Helper
        /// </summary>
        protected ApinationAPI _apinationApi => new ApinationAPI();
    }
}