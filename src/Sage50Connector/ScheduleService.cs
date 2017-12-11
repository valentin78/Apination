using System;
using System.Diagnostics;
using System.Reflection;
using System.ServiceProcess;
using log4net;
using log4net.Config;
using Sage50Connector.API;
using Sage50Connector.Core;
using Sage50Connector.Processing;
using Sage50Connector.Processing.Actions;
using Sage50Connector.Processing.HeartBeat;

namespace Sage50Connector
{
    public partial class ScheduleService : ServiceBase
    {
        public static readonly ILog Log = LogManager.GetLogger(typeof(ScheduleService));
        public static void InitializeLogger() { XmlConfigurator.Configure(); }

        static ScheduleService()
        {
            InitializeLogger();
        }

        public ScheduleService()
        {
            InitializeComponent();
        }

        private HeartBeatReporter heartBeatProcessor;
        private SageActionsProcessor sageActionsProcessor;


        protected override void OnStart(string[] args)
        {
            Log.Info("********************************************************************************************************************");
            Log.Info("* Sage50Connector Service starting");
            Log.Info("********************************************************************************************************************");

            try
            {

                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

                // retrieve config
                Log.Info("Retrieve Connector Config ...");
                var config = new ApinationApi(new WebClientHttpUtility(), config: null).GetConnectorConfig();
                Log.InfoFormat("Received Config: {0}", config);

                StartupHandler.OnStartup();

                heartBeatProcessor = new HeartBeatReporter();
                heartBeatProcessor.StartHeartBeatReporting(config);
                sageActionsProcessor = new SageActionsProcessor();
                sageActionsProcessor.StartActionsProcessing(config);

            }
            catch (Exception ex)
            {
                Log.FatalFormat("Sage50Connector Service start failed: {0}. Trace: {1}", ex.Message, ex);

                // if loader exception throw, log exception with loaderException details
                if (ex is ReflectionTypeLoadException loaderException)
                    loaderException.LogLoaderExceptions((e, le) => Log.FatalFormat("Loader Exception: {0}. Trace: {1}", le.Message, le));

                // stopping service
                Stop();
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Info("********************************************************************************************************************");
            Log.FatalFormat("* UnhandledException: ExceptionObject = {0}, IsTerminating = {1}", e.ExceptionObject, e.IsTerminating);
            Log.Info("********************************************************************************************************************");
        }

        protected override void OnStop()
        {
            heartBeatProcessor?.Dispose();
            sageActionsProcessor?.Dispose();

            Log.Info("********************************************************************************************************************");
            Log.Info("* Sage50Connector Service stopped");
            Log.Info("********************************************************************************************************************");
        }
    }
}
