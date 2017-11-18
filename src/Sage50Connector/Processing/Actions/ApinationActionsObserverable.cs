using System;
using System.Collections.Generic;
using log4net;
using Quartz;
using Quartz.Impl.Matchers;
using Sage50Connector.Models;
using Sage50Connector.Processing.Actions.SageActions;

namespace Sage50Connector.Processing.Actions
{
    /// <summary>
    /// Supports actions processing
    /// </summary>
    class SageActionsObserverable : IObservable<IEnumerable<SageAction>>
    {
        public static readonly ILog Log = LogManager.GetLogger(typeof(SageActionsObserverable));

        private Action<IEnumerable<SageAction>> subscriber;
        private readonly IJobDetail job;
        private readonly ITrigger trigger;
        private readonly IScheduler scheduler;
        private readonly PollApinationJobListener apinationListener;
        private Config config;

        public SageActionsObserverable(
            IJobDetail job,
            ITrigger trigger,
            IScheduler scheduler,
            PollApinationJobListener apinationListener,
            Config config)
        {
            this.job = job;
            this.trigger = trigger;
            this.scheduler = scheduler;
            this.apinationListener = apinationListener;
            this.config = config;
        }

        private IApinationListener StartApinationPolling()
        {
            scheduler.ListenerManager.AddJobListener(apinationListener, KeyMatcher<JobKey>.KeyEquals(job.Key));
            scheduler.ScheduleJob(job, trigger);
            
            scheduler.TriggerJob(job.Key);
            
            return apinationListener;
        }

        /// <summary>
        /// Set subscriber to handle OnNewSageAction event
        /// 
        /// Assumes only one subscriber for now, bacause the lack of need to support multiple
        /// Accepts a function that accepts a list of Sage50 Actions (that came from Apination)
        /// </summary>
        /// <param name="subscriber"></param>
        // ReSharper disable once ParameterHidesMember
        public void Subscribe(Action<IEnumerable<SageAction>> subscriber)
        {
            // Prevents multiple subscription
            if (this.subscriber != null) { throw new NotSupportedException(); }

            this.subscriber = subscriber;
            // ReSharper disable once LocalVariableHidesMember
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
}