//using Common.Logging;
using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ApinationGateway
{
    public partial class SheduleService : ServiceBase
    {
        #region Logger

        private static readonly ILog log = LogManager.GetLogger(typeof(SheduleService));
        public static ILog Log { get { return log; } }
        public static void InitializeLogger() { XmlConfigurator.Configure(); }

        #endregion

        static SheduleService()
        {
            InitializeLogger();
        }

        public SheduleService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Log.Info("OnStart");
        }

        protected override void OnStop()
        {
            Log.Info("OnStop");
        }
    }
}
