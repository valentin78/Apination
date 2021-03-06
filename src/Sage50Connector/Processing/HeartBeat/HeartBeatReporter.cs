﻿using System;
using log4net;
using Quartz;
using Quartz.Impl;
using Sage50Connector.API;
using Sage50Connector.Core;
using Sage50Connector.Models;

namespace Sage50Connector.Processing.HeartBeat
{
    /// <summary>
    /// HeartBeat Processor
    /// </summary>
    class HeartBeatReporter : IDisposable
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(HeartBeatReporter));

        private IScheduler scheduler;

        public void StartHeartBeatReporting(Config config)
        {
            Log.InfoFormat("HeartBeatProcessor running with config: '{0}'", config);

            try
            {
                var apinationApi = new ApinationApi(new WebClientHttpUtility(), config);
                
                IJobDetail heartBeatJob = JobBuilder.Create<HeartBeatJob>()
                    .WithIdentity("HeartBeatJob")
                    .Build();

                var cronTrigger = TriggerBuilder.Create()
                    .StartNow()
                    .WithCronSchedule(config.HeartBeatCronSchedule)
                    .Build();

                var schedulerFactory = new StdSchedulerFactory();
                scheduler = schedulerFactory.GetScheduler();
                scheduler.JobFactory = new JobFactory(apinationApi);

                scheduler.ScheduleJob(heartBeatJob, cronTrigger);

                scheduler.TriggerJob(heartBeatJob.Key);

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

