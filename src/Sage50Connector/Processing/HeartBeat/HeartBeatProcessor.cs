using System;
using log4net;
using Quartz;
using Quartz.Impl;
using Sage.Peachtree.API.Factories;
using Sage50Connector.API;
using Sage50Connector.Core;
using Sage50Connector.Models;

namespace Sage50Connector.Processing.HeartBeat
{
    /// <summary>
    /// HeartBeat Processor
    /// </summary>
    class HeartBeatProcessor : IDisposable
    {
        public static readonly ILog Log = LogManager.GetLogger(typeof(HeartBeatProcessor));

        private IScheduler scheduler;

        public void StartHeartBeat(Config config)
        {
            Log.InfoFormat("HeartBeatProcessor running with config: '{0}'", config);

            try
            {
                var apinationApi = new ApinationApi(new WebClientHttpUtility());
                
                IJobDetail pollApinationJob = JobBuilder.Create<HeartBeatJob>()
                    .WithIdentity("HeartBeatJob")
                    .Build();

                var cronTrigger = TriggerBuilder.Create()
                    .StartNow()
                    .WithCronSchedule(config.HeartBeatCronSchedule)
                    .Build();

                var schedulerFactory = new StdSchedulerFactory();
                scheduler = schedulerFactory.GetScheduler();
                scheduler.JobFactory = new JobFactory(apinationApi);

                scheduler.ScheduleJob(pollApinationJob, cronTrigger);

                scheduler.TriggerJob(pollApinationJob.Key);

                scheduler.Start();
            }
            catch (Exception ex)
            {
                Log.Error("Unknown Exception received: ", ex);
            }
        }

        public void Dispose()
        {
            scheduler?.Shutdown();
        }
    }
}

