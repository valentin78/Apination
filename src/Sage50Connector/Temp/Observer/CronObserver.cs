using System;
using log4net;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using Sage50Connector.Temp.Observer.Interface;
using Sage50Connector.Temp.Workflows;

namespace Sage50Connector.Temp.Observer
{
    /// <summary>
    /// Observer based on CronScheduler
    /// </summary>
    class CronObserver : IObserver
    {
        public static readonly ILog Log = LogManager.GetLogger(typeof(CronObserver));

        // ReSharper disable once InconsistentNaming
        private readonly CronScheduleFabrik.JobListener listener;
        
        private readonly CronScheduleFabrik cronScheduleFabrik;

        public string Identity => listener.JobIdentityName;

        public CronObserver(string cronPeriod)
        {
            cronScheduleFabrik = new CronScheduleFabrik();
            listener = cronScheduleFabrik.Create(cronPeriod);
        }

        public delegate void DataHandler(EventData data, EventArgs e);
        private event DataHandler OnDataEvent;

        public void TriggerOnDataEvent(EventData data)
        {
            OnDataEvent?.Invoke(data, EventArgs.Empty);
        }

        public void AddChecker(IChecker checker)
        {
            Log.Info("=> Register checker: " + listener.JobIdentityName);

            var o = this;
            listener.OnExecuted += (sender, args) => {
                try
                {
                    checker.Check(o);
                }
                catch (Exception exc)
                {
                    Log.Debug("!CHECKING ERROR: {0}", exc);
                }
            };
        }

        public void Subscribe(ISaver saver)
        {
            OnDataEvent += (data, args) =>
            {
                if (data.Type == saver.PayloadTypeFiler)
                {
                    saver.Save(data);
                    //Log.InfoFormat("OnDataEvent raised with Data: {0};", data);
                }
            };
        }

        public void Dispose()
        {
            cronScheduleFabrik.Dispose();
        }
    }
    
    /// <summary>
    /// Fabrik Type for JobListener instance creation
    /// </summary>
    class CronScheduleFabrik : IDisposable
    {
        public static readonly ILog Log = LogManager.GetLogger(typeof(CronScheduleFabrik));
        private IScheduler scheduler;
        private readonly string JobIdentityName = Guid.NewGuid().ToString();

        public JobListener Create(string cronPeriod)
        {
            Log.InfoFormat("Statring Scheduler for cronPeriod: {0}; JobIdentityName: {1}", cronPeriod, JobIdentityName);

            var schedulerFactory = new StdSchedulerFactory();
            scheduler = schedulerFactory.GetScheduler();

            scheduler.Start();

            var job = JobBuilder.Create<ScheduledProcess>()
                .WithIdentity(JobIdentityName)
                .Build();

            var trigger = TriggerBuilder.Create().WithIdentity(JobIdentityName)
                .StartNow()
                .WithCronSchedule(cronPeriod)
                .Build();

            scheduler.ScheduleJob(job, trigger);

            var jobListener = new JobListener(JobIdentityName);

            scheduler.ListenerManager.AddJobListener(jobListener, KeyMatcher<JobKey>.KeyEquals(new JobKey(JobIdentityName)));
            return jobListener;
        }

        public void Dispose()
        {
            scheduler.Shutdown(waitForJobsToComplete: true);
        }

        /// <summary>
        /// Required type for running Job
        /// </summary>
        [DisallowConcurrentExecution]
        // ReSharper disable once ClassNeverInstantiated.Local
        private class ScheduledProcess : IJob
        {
            public void Execute(IJobExecutionContext context)
            {
            }
        }

        /// <summary>
        /// JobListener realization
        /// </summary>
        public class JobListener : IJobListener
        {
            public static readonly ILog Log = LogManager.GetLogger(typeof(CronScheduleFabrik));

            /// <summary>
            /// technical jobName for debuggind & logging purposes
            /// </summary>
            public string JobIdentityName { get; private set; }

            public JobListener(string jobIdentityName)
            {
                JobIdentityName = jobIdentityName;
            }

            public event EventHandler OnExecuted;

            public void JobToBeExecuted(IJobExecutionContext context) { }

            public void JobExecutionVetoed(IJobExecutionContext context) { }

            public void JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException)
            {
                //Log.InfoFormat("JobWasExecuted; JobIdentityName: {0}", JobIdentityName);
                OnExecuted?.Invoke(this, EventArgs.Empty);
            }

            public string name = Guid.NewGuid().ToString();
            public string Name => name;
        }
    }
}