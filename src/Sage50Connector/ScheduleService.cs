using System;
using System.Collections.Generic;
using System.Reflection;
using System.ServiceProcess;
using log4net;
using log4net.Config;
using Quartz;
using Quartz.Impl;
using Sage50Connector.API;
using Sage50Connector.Core;
using Sage50Connector.HeartBeat;
using Sage50Connector.Models;
using Sage50Connector.Processes;

namespace Sage50Connector
{
    public partial class ScheduleService : ServiceBase
    {
        /// <summary>
        /// Apination Api Util
        /// </summary>
        private ApinationApi _apinationApi => new ApinationApi(new WebClientHttpUtility());

        /// <summary>
        /// Sage50 Api
        /// </summary>
        private Sage50Api _sage50Api => new Sage50Api();

        readonly Lazy<IScheduler> _scheduler = new Lazy<IScheduler>(() =>
        {
            var schedulerFactory = new StdSchedulerFactory();
            return schedulerFactory.GetScheduler();
        }, true);

        private IScheduler Scheduler => _scheduler.Value;

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

        /// <summary>
        /// Schedule generic Observer process
        /// </summary>
        private void ScheduleObserver<TObserver>(string cronSchedule, Config config) where TObserver : ProcessBase
        {
            if (string.IsNullOrEmpty(cronSchedule)) throw new ArgumentException("Cron Schedule not specified", nameof(cronSchedule));

            var job = JobBuilder.Create<TObserver>().Build();

            IDictionary<string, object> jobDataMap = new Dictionary<string, object>
            {
                { "Config", config },
                { "Sage50Api", _sage50Api},
                { "ApinationApi", _apinationApi},
            };

            var jobData = new JobDataMap(jobDataMap);

            var trigger = TriggerBuilder.Create()
                .StartNow()
                .UsingJobData(jobData)
                .WithCronSchedule(cronSchedule).Build();

            Scheduler.ScheduleJob(job, trigger);

            // auto start process on service started
            Scheduler.TriggerJob(job.Key);
        }

        /// <summary>
        /// Schedule HeartBeat process
        /// </summary>
        private void ScheduleHeartBeat(string cronSchedule)
        {
            // if not specified HeartBeatCronSchedule skip this schedule
            if (string.IsNullOrEmpty(cronSchedule)) return;

            var job = JobBuilder.Create<HeartBeatProcess>().Build();

            var trigger = TriggerBuilder.Create()
                .StartNow()
                .WithCronSchedule(cronSchedule).Build();

            Scheduler.ScheduleJob(job, trigger);
        }

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


                // start schedules
                Scheduler.Start();

                Log.Info("Schedule and start Sage50 Observer ...");
                ScheduleObserver<Sage50Observer>(config.Sage50CronSchedule, config);
                
                Log.Info("Schedule and start Apination Observer ...");
                ScheduleObserver<ApinationObserver>(config.ApinationCronSchedule, config);

                ScheduleHeartBeat(config.HeartBeatCronSchedule);
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
            // shutdown scheduler jobs and main process and waiting for finish them 
            Scheduler.Shutdown(true);

            Log.Info("********************************************************************************************************************");
            Log.Info("* Sage50Connector Service stopped");
            Log.Info("********************************************************************************************************************");
        }
    }
}
