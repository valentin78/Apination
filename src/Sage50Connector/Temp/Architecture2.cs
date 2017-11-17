using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using Sage50Connector.API;
using Sage50Connector.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sage50Connector.Temp
{
    /*
      In this update I renamed "Observer" to "Observable" to loosely follow Reactive Extentions
      http://reactivex.io/documentation/observable.html
      and this patter descritpion: https://docs.microsoft.com/en-us/dotnet/standard/events/observer-design-pattern

      I did this because it's more common to call "something that produces events" as "observable"
      and not "observer".

      To sum up:

      What used to be Observer is now Observable.
      Subscriver is still subscriber, just to be explicit of the differences from above pattern.

      For simplicity, I didn't implement separate class that implements "IObserver" as per
      that pattern. Instead observer(subscriber) is just a function, but this can be refactored later
      if we ever decide we need it.
     */

    // interface IJobWithSubscription : IJob { void Subscribe(Action subscriber); }

    interface IObservable<T> { void Subscribe(Action<T> action); }

    class PollApinationJob : IJob
    {
        //  custom constructor will require a custom Job factory that has to be registered
        //  with Quartz
        public PollApinationJob(ApinationApi api) { }

        public void Execute(IJobExecutionContext context)
        {
            // connects to Apination API, stores actions in context.JobDetail.JobDataMap
            context.JobDetail.JobDataMap.Add("actions", new object[] { new SageActionDTO { Type = "type", Payload = "payload" } });
            throw new NotImplementedException();
        }
    }

    interface IApinationListener
    {
        event EventHandler<IList<SageAction>> OnNewSageActions;
    }

    // implements IJobLIstener interface so it can be used in Quartz,
    // but also implements IApinationListener interface representing arrival of
    // the action from apination. The closest analgoy in your propsal is "Checker".
    public class PollApinationJobListener : IJobListener, IApinationListener
    {
        private ISageActionFactory actionFactory;

        public PollApinationJobListener(ISageActionFactory actionFactory)
        {
            this.actionFactory = actionFactory;
        }

        public string Name => throw new NotImplementedException();

        public event EventHandler<IList<SageAction>> OnNewSageActions;

        public void JobExecutionVetoed(IJobExecutionContext context)
        {
        }

        public void JobToBeExecuted(IJobExecutionContext context)
        {
        }

        public void JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException)
        {

            if (context.JobDetail.JobDataMap["actions"] == null) return;

            IList<SageAction> sageActions = ((SageActionDTO[])context.JobDetail.JobDataMap["actions"])
              // Action factory is where action creation would happen based on string Type,
              // so it's wehre that bit "switch" would happen that we discussed here:
              // https://github.com/Apination/sage50-windows-service/pull/2#discussion_r150689441
              .Select(actionDto => actionFactory.Create(actionDto)).ToList();
            NewSageActions(sageActions);

        }

        protected virtual void NewSageActions(IList<SageAction> actions)
        {
            OnNewSageActions?.Invoke(this, actions);
        }

    }

    public interface ISageActionFactory
    {
        SageAction Create(SageActionDTO dto);
    }

    // Observes Apination for actions, abstracts away implementation details of the
    // "observation process" by using polling mechanism internally using Quartz Scheduler
    // Incapsulates how to do polling, how often (configurable) etc. Uses Quartz Job Listener
    // to react on Job execution event that figures out if job got actions or not.
    //
    // Does NOT make any assumptions about subscribers, they are just functions.
    class ApinationActionsObserverable : IObservable<IList<SageAction>>
    {
        // Not sure what is the exact QUartz type name for Job Trigger
        private ITrigger jobTrigger;
        private Action<IList<SageAction>> subscriber;
        private IJobDetail job; // allows to define job separately
        private ITrigger trigger; //allows to define schedule separately
        private IScheduler scheduler;
        private PollApinationJobListener apinationListener;
        private Config config;

        public ApinationActionsObserverable(
         IJobDetail job, // allows to define job separately
         ITrigger trigger, //allows to define schedule separately
         IScheduler scheduler,
         PollApinationJobListener apinationListener,
         Config config)
        { /* set params to this.param = */ }

        // If needed, this could be extracted from this class to it's own Type,
        private IApinationListener StartApinationPolling()
        {
            // in reality this method is a little different, just an example
            scheduler.ListenerManager.AddJobListener(apinationListener, KeyMatcher<JobKey>.KeyEquals(new JobKey("PollApinationJob")));
            scheduler.ScheduleJob(job, trigger);
            return apinationListener;
        }

        // assumes only one subscriber for now, bacause the lack of need
        // to support multiple, accepts a function that accepts a list of Sage50 Actions
        // (that came from Apination)
        public void Subscribe(Action<IList<SageAction>> subscriber)
        {
            this.subscriber = subscriber;

            if (this.subscriber != null) { throw new NotSupportedException(); }

            IApinationListener apinationListener = StartApinationPolling();
            apinationListener.OnNewSageActions += (sender, actions) =>
            {
                // send all actions at once, so if multiple actions came up
                // subscriber can decide how to deal with this situation and in which order to
                // handle them
                this.subscriber(actions);
            };
        }

    }


    class Startup
    {
        public void StartPollApination(Config config)
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

            // from the following observable definition it's immediately obvious that
            // it's composed from job, particular cron trigger, scheduler and listener
            ApinationActionsObserverable apinationObservable = new ApinationActionsObserverable(
          job: pollApinationJob,
          trigger: cronTrigger,
          scheduler: scheduler,
          apinationListener: new PollApinationJobListener(new ActionFactory()),
          config: config

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
        }
    }

    public class SageActionDTO
    {
        public string Type { get; set; }
        public string Payload { get; set; }
    }

    public class SageAction
    {
    }

    class ActionFactory : ISageActionFactory
    {
        public SageAction Create(SageActionDTO dto)
        {
            throw new NotImplementedException();
        }
    }

    public class SageActionHandlerFactory
    {
        public static ISageActionHandler CreateHandler(SageAction action)
        {
            throw new NotImplementedException();
        }
    }

   public interface ISageActionHandler
    {
        void Handle(SageAction action);
    }
}

