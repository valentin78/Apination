using System;
using System.Collections.Generic;
using System.Reflection;
using log4net;
using System.ServiceProcess;
using ApinationGateway.API;
using ApinationGateway.Core;
using ApinationGateway.Models;
using ApinationGateway.Processes;
using log4net.Config;
using Quartz;
using Quartz.Impl;

namespace ApinationGateway
{
    public partial class SheduleService : ServiceBase
    {
        private ApinationAPI _apinationApi => new ApinationAPI();
        private Config _config;

        #region Scheduler

        readonly Lazy<IScheduler> _scheduler = new Lazy<IScheduler>(() =>
        {
            var schedulerFactory = new StdSchedulerFactory();
            return schedulerFactory.GetScheduler();
        }, true);

        private IScheduler Scheduler => _scheduler.Value;

        #endregion

        #region Logger

        public static readonly ILog Log = LogManager.GetLogger(typeof(SheduleService));
        public static void InitializeLogger() { XmlConfigurator.Configure(); }

        #endregion

        static SheduleService()
        {
            InitializeLogger();
        }

        public SheduleService()
        {
            InitializeComponent();

            _config = _apinationApi.RetrieveGatewayConfig();
        }

        protected override void OnStart(string[] args)
        {
            Log.Info("********************************************************************************************************************");
            Log.Info("* ApinationGateway Service starting");
            Log.Info("********************************************************************************************************************");

            try
            {
                var dictionary = new Dictionary<IJobDetail, Quartz.Collection.ISet<ITrigger>>();

                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                Scheduler.Start();

                var job = JobBuilder.Create<SampleProcess>().Build();

                var trigger = TriggerBuilder.Create()
                    .StartNow()
                    .WithCronSchedule("0/1 * * * * ?") //every second
                    .Build();

                dictionary.Add(job, new Quartz.Collection.HashSet<ITrigger> { trigger });
                Scheduler.ScheduleJobs(dictionary, true);

                Scheduler.TriggerJob(job.Key);
            }
            catch (Exception exc)
            {
                Log.FatalFormat("ApinationGateway Service start failed: {0}. Trace: {1}", exc.Message, exc);

                // if loader exception throw, log exception with loaderException details
                if (exc is ReflectionTypeLoadException loaderException)
                    loaderException.LogLoaderExceptions((e, le) => Log.FatalFormat("Loader Exception: {0}. Trace: {1}", le.Message, le));

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
            Scheduler.Shutdown(true);

            Log.Info("********************************************************************************************************************");
            Log.Info("* ApinationGateway Service stopped");
            Log.Info("********************************************************************************************************************");
        }
    }
}
