using Common.Logging;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ApinationGateway
{

    static class Program
    {
        #region Logger

        private static readonly ILog log = LogManager.GetLogger(typeof(Program));
        public static ILog Log { get { return log; } }
        public static void InitializeLogger() { XmlConfigurator.Configure(); }

        #endregion
        /// <summary>
        /// The main entry point for the application
        /// </summary>
        static void Main()
        {
            InitializeLogger();
            Log.Error("Program Started ...");

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new SheduleService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
