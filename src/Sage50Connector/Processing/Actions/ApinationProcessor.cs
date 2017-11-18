using System;
using System.Linq;
using log4net;
using Quartz;
using Quartz.Impl;
using Sage50Connector.Models;
using Sage50Connector.Processing.Actions.ActionHandlers;
using Sage50Connector.Processing.Actions.ActionHandlers.Factory;
using Sage50Connector.Processing.Actions.SageActions.Factory;

namespace Sage50Connector.Processing.Actions
{
    class ApinationProcessor : IDisposable
    {
        public static readonly ILog Log = LogManager.GetLogger(typeof(ApinationProcessor));

        private IScheduler scheduler;

        public SageActionsObserverable StartPollApination(Config config)
        {
            Log.InfoFormat("StartPollApination running width config: '{0}'", config);

            try
            {
                // Observable creation
                IJobDetail pollApinationJob = JobBuilder.Create<PollApinationJob>()
                    .WithIdentity("PollApinationJob")
                    .Build();

                var cronTrigger = TriggerBuilder.Create()
                    .StartNow()
                    .WithCronSchedule(config.ApinationCronSchedule)
                    .Build();

                var schedulerFactory = new StdSchedulerFactory();
                scheduler = schedulerFactory.GetScheduler();
                scheduler.JobFactory = new PollApinationJobFactory();

                // from the following observable definition it's immediately obvious that
                // it's composed from job, particular cron trigger, scheduler and listener
                SageActionsObserverable apinationObservable = new SageActionsObserverable(
                    job: pollApinationJob,
                    trigger: cronTrigger,
                    scheduler: scheduler,
                    apinationListener: new PollApinationJobListener(new SageActionJsonFactory()),
                    config: config
                );

                //and for apination
                apinationObservable.Subscribe(actions =>
                {
                    // actions can be handled in any order, this is the right place to put this logic
                    // most of the time it will be just 1-1 action to handler assocciation
                    // ActionHandlers are what you call "Savers", but for actions
                    try
                    {
                        foreach (var action in actions)
                        {
                            ISageActionHandler handler = SageActionHandlerFactory.CreateHandler(action);
                            handler.Handle(action);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Actions processing error: ", ex);
                    }
                });

                scheduler.Start();

                return apinationObservable;
            }
            catch (Exception ex)
            {
                Log.Error("Unknown Exception received: ", ex);
                return null;
            }
        }

        public void Dispose()
        {
            scheduler?.Shutdown();
        }
    }
}

