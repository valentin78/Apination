using System.Linq;
using Quartz;
using Quartz.Impl;
using Sage50Connector.Models;
using Sage50Connector.Processing.Actions.ActionHandlers;
using Sage50Connector.Processing.Actions.ActionHandlers.Factory;
using Sage50Connector.Processing.Actions.SageActions.Factory;

namespace Sage50Connector.Processing.Actions
{
    // Observes Apination for actions, abstracts away implementation details of the
    // "observation process" by using polling mechanism internally using Quartz Scheduler
    // Incapsulates how to do polling, how often (configurable) etc. Uses Quartz Job Listener
    // to react on Job execution event that figures out if job got actions or not.
    //
    // Does NOT make any assumptions about subscribers, they are just functions.


    class Startup
    {
        public static SageActionsObserverable StartPollApination(Config config)
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
            var scheduler = schedulerFactory.GetScheduler();
            scheduler.JobFactory = new PollApinationJobFactory();

            // from the following observable definition it's immediately obvious that
            // it's composed from job, particular cron trigger, scheduler and listener
            SageActionsObserverable apinationObservable = new SageActionsObserverable(
                    job: pollApinationJob,
                    trigger: cronTrigger,
                    scheduler: scheduler,
                    apinationListener: new PollApinationJobListener(new SageActionJsonFactory()), config: config
                );

            //and for apination
            apinationObservable.Subscribe(actions =>
            {
                // actions can be handled in any order, this is the right place to put this logic
                // most of the time it will be just 1-1 action to handler assocciation
                // ActionHandlers are what you call "Savers", but for actions
                actions.Select(action =>
                {
                    ISageActionHandler handler = SageActionHandlerFactory.CreateHandler(action);
                    handler.Handle(action);
                    return handler;
                });
            });

            scheduler.Start();

            return apinationObservable;
        }
    }
}

