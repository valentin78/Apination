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
using QuartzTriggerSet = Quartz.Collection.ISet<Quartz.ITrigger>;

namespace Sage50Connector
{
    public partial class ScheduleService : ServiceBase
    {
        // Jobs store
        private readonly Dictionary<IJobDetail, QuartzTriggerSet> _jobsStore = new Dictionary<IJobDetail, QuartzTriggerSet>();
        // auto start job keys list
        private readonly List<JobKey> _jobsAutoStart = new List<JobKey>();

        /// <summary>
        /// Apination Api Util
        /// </summary>
        private ApinationApi _apinationApi => new ApinationApi(new WebClientHttpUtility());
        
        // Connector Config
        private Config _config;

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
        /// Prepares job for SyncProcess config
        /// </summary>
        /// <param name="process"></param>
        /// <param name="company"></param>
        void ScheduleProcess(Process process, Company company)
        {
            var jobType = ProcessesUtil.GetProcessTypeLocatorById(process.ProcessId);
            if (jobType == null)
            {
                Log.ErrorFormat("--- Error: Not located process with ID '{0}'", process.ProcessId);
                return;
            }

            var cron = process.CronSchedule ?? _config.DefaultCronSchedule;
            var autoStart = process.AutoStart;

            process.ProcessParams.Add("$company", company);
            // add job custom data for process needs
            var jobData = new JobDataMap(process.ProcessParams);

            var job = JobBuilder.Create(jobType)
                .UsingJobData(jobData)
                .Build();

            var trigger = TriggerBuilder.Create()
                .StartNow()
                .WithCronSchedule(cron).Build();
            _jobsStore.Add(job, new Quartz.Collection.HashSet<ITrigger> { trigger });

            if (autoStart) _jobsAutoStart.Add(job.Key);
        }
        /// <summary>
        /// Schedule HeartBeat process
        /// </summary>
        private void ScheduleHeartBeat()
        {
            // if not specified HeartBeatCronSchedule skip this schedule
            if (string.IsNullOrEmpty(_config.HeartBeatCronSchedule)) return;

            var job = JobBuilder.Create<HeartBeatProcess>().Build();

            var trigger = TriggerBuilder.Create()
                .StartNow()
                .WithCronSchedule(_config.HeartBeatCronSchedule).Build();

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
                _config = _apinationApi.RetrieveConnectorConfig();

                Log.InfoFormat("Default Cron config: {0}", _config.DefaultCronSchedule);

                // for every company and their processes prepare jobs in job strore
                foreach (var company in _config.CompaniesList)
                {
                    Log.InfoFormat("| Company '{0}' ...", company.CompanyName);
                    foreach (var process in company.Processes)
                    {
                        Log.InfoFormat("| - Process config: '{0}'", process);
                        ScheduleProcess(process, company);
                    }
                }

                // start schedules
                Scheduler.Start();

                ScheduleHeartBeat();

                // start schedule jobs from jobs store
                Scheduler.ScheduleJobs(_jobsStore, true);

                // auto start jobs
                foreach (var jobKey in _jobsAutoStart) Scheduler.TriggerJob(jobKey);
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
