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
        private Dictionary<IJobDetail, Quartz.Collection.ISet<ITrigger>> _jobsStore = new Dictionary<IJobDetail, Quartz.Collection.ISet<ITrigger>>();
        List<JobKey> _jobsAutoStart = new List<JobKey>();

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
        }

        void ScheduleProcess(SyncProcess process)
        {
            var jobType = Helpers.ProcessTypeLocator(process.ProcessID);
            var cron = process.CronSchedule;
            var autoStart = process.AutoStart;

            var job = JobBuilder.Create(jobType).Build();
            var trigger = TriggerBuilder.Create().StartNow().WithCronSchedule(cron).Build();
            _jobsStore.Add(job, new Quartz.Collection.HashSet<ITrigger> { trigger });

            if (autoStart) _jobsAutoStart.Add(job.Key);
        }

        protected override void OnStart(string[] args)
        {
            Log.Info("********************************************************************************************************************");
            Log.Info("* ApinationGateway Service starting");
            Log.Info("********************************************************************************************************************");

            try
            {
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

                Log.Info("Retrieve Gateway Config ...");
                _config = _apinationApi.RetrieveGatewayConfig();

                foreach (var company in _config.CompaniesList)
                {
                    Log.InfoFormat("* Company '{0}' ...", company.CompanyName);
                    foreach (var process in company.Processes)
                    {
                        Log.InfoFormat("** Process config: '{0}'", process);
                        ScheduleProcess(process);
                    }
                }

                Scheduler.Start();

                Scheduler.ScheduleJobs(_jobsStore, true);
                foreach (var jobKey in _jobsAutoStart) Scheduler.TriggerJob(jobKey);
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
