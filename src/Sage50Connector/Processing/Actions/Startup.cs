using System.Linq;
using Quartz;
using Quartz.Impl;
using Sage50Connector.Models;
using Sage50Connector.Processing.Actions.ActionHandlers;
using Sage50Connector.Processing.Actions.ActionHandlers.Factory;
using Sage50Connector.Processing.Actions.SageActions.Factory;

namespace Sage50Connector.Processing.Actions
{

    // TODO Do something with this
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

            SageActionsObserverable sageApinationObservable = new SageActionsObserverable(
                    job: pollApinationJob,
                    trigger: cronTrigger,
                    scheduler: scheduler,
                    apinationJobListener: new PollApinationJobListener(new SageActionJsonFactory()), config: config
                );

            // for apination
            sageApinationObservable.Subscribe(sageActions =>
            {
                // actions can be handled in any order, this is the right place to put this logic
                // most of the time it will be just 1-1 action to handler assocciation
                foreach (var sageAction in sageActions)
                {
                    var handler = SageActionHandlerFactory.CreateHandler(sageAction);
                    handler.Handle(sageAction);
                }
            });

            scheduler.Start();

            return sageApinationObservable;
        }
    }
}

