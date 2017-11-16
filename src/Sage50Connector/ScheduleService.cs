using System;
using System.Collections.Generic;
using System.Reflection;
using System.ServiceProcess;
using log4net;
using log4net.Config;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using Sage.Peachtree.API.Factories;
using Sage50Connector.API;
using Sage50Connector.Core;
using Sage50Connector.HeartBeat;
using Sage50Connector.Models;
using Sage50Connector.Processing;
using Sage50Connector.Temp;

namespace Sage50Connector
{
    public partial class ScheduleService : ServiceBase
    {
        /// <summary>
        /// Apination Api Util 
        /// </summary>
        private ApinationApi _apinationApi => new ApinationApi(new WebClientHttpUtility());
        
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

        private IObserver apinationObserver;
        private IObserver sage50Observer;

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
                var config = _apinationApi.RetrieveConnectorConfig();
                Log.InfoFormat("Received Config: {0}", config);


                apinationObserver = new ApinationObserverFabrik().Create(config);
                sage50Observer = new Sage50ObserverFabrik().Create(config);


                //Log.Info("Schedule and start Sage50 Observer ...");
                //ScheduleObserver<Sage50Observer>(config.Sage50CronSchedule, config);
                
                //Log.Info("Schedule and start Apination Observer ...");
                //ScheduleObserver<ApinationObserver>(config.ApinationCronSchedule, config);

                //ScheduleHeartBeat(config.HeartBeatCronSchedule);
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
            sage50Observer.Dispose();
            apinationObserver.Dispose();

            Log.Info("********************************************************************************************************************");
            Log.Info("* Sage50Connector Service stopped");
            Log.Info("********************************************************************************************************************");
        }
    }
}
