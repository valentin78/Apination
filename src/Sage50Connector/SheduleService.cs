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
using Sage50Connector.Models;

namespace Sage50Connector
{
    public partial class SheduleService : ServiceBase
    {
        // jobs store
        private readonly Dictionary<IJobDetail, Quartz.Collection.ISet<ITrigger>> _jobsStore = new Dictionary<IJobDetail, Quartz.Collection.ISet<ITrigger>>();
        // auto start job keys list
        private readonly List<JobKey> _jobsAutoStart = new List<JobKey>();

        // Apination API Helper
        private ApinationAPI _apinationApi => new ApinationAPI();
        
        // Connector Config
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
        }

        /// <summary>
        /// prepare job for SyncProcess config
        /// </summary>
        /// <param name="process"></param>
        void ScheduleProcess(SyncProcess process)
        {
            var jobType = Helpers.ProcessTypeLocator(process.ProcessID);
            if (jobType == null)
            {
                Log.ErrorFormat("--- Error: Not located process with ID '{0}'", process.ProcessID);
                return;
            }

            var cron = process.CronSchedule ?? _config.DefaultCronSchedule;
            var autoStart = process.AutoStart;

            // add job custom data for process needs
            var jobData = new JobDataMap(process.JobData);

            var job = JobBuilder.Create(jobType)
                .UsingJobData(jobData)
                .Build();

            var trigger = TriggerBuilder.Create()
                .StartNow()
                .WithCronSchedule(cron).Build();
            _jobsStore.Add(job, new Quartz.Collection.HashSet<ITrigger> { trigger });

            if (autoStart) _jobsAutoStart.Add(job.Key);
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
                _config = _apinationApi.RetrieveConnectorConfig();

                Log.InfoFormat("Default Cron config: {0}", _config.DefaultCronSchedule);

                // for every company and their processes prepare jobs in job strore
                foreach (var company in _config.CompaniesList)
                {
                    Log.InfoFormat("| Company '{0}' ...", company.CompanyName);
                    foreach (var process in company.Processes)
                    {
                        Log.InfoFormat("| - Process config: '{0}'", process);
                        ScheduleProcess(process);
                    }
                }

                // start schedules
                Scheduler.Start();

                // start schedule jobs from jobs store
                Scheduler.ScheduleJobs(_jobsStore, true);

                // auto start jobs
                foreach (var jobKey in _jobsAutoStart) Scheduler.TriggerJob(jobKey);
            }
            catch (Exception exc)
            {
                Log.FatalFormat("Sage50Connector Service start failed: {0}. Trace: {1}", exc.Message, exc);

                // if loader exception throw, log exception with loaderException details
                if (exc is ReflectionTypeLoadException loaderException)
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
