using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using Quartz;
using Sage50Connector.Processing.Actions.SageActions;
using Sage50Connector.Processing.Actions.SageActions.Factory;

namespace Sage50Connector.Processing.Actions
{
    /// <summary>
    /// Handle JobWasExecuted from PollApinationJob
    /// If new Actions detected, throws OnNewSageActions event
    /// </summary>
    public class PollApinationJobListener : IJobListener, IApinationListener
    {
        public static readonly ILog Log = LogManager.GetLogger(typeof(PollApinationJobListener));

        // ReSharper disable once InconsistentNaming
        private readonly ISageActionFactory actionFactory;

        public PollApinationJobListener(ISageActionFactory actionFactory)
        {
            this.actionFactory = actionFactory;
        }

        public string Name => Guid.NewGuid().ToString();

        public event EventHandler<IEnumerable<SageAction>> OnNewSageActions;

        public void JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException)
        {
            if (context.JobDetail.JobDataMap["actions"] == null) return;

            var sageActionsJson = ((IEnumerable<string>)context.JobDetail.JobDataMap["actions"]);
            
            Log.Info("Creating Sage50 actions from JSON ...");
            var sageActions = sageActionsJson.Select(actionJson => actionFactory.Create(actionJson));

            NewSageActions(sageActions);
        }

        protected virtual void NewSageActions(IEnumerable<SageAction> actions)
        {
            Log.Info("Trigger NewSageActions event ...");
            OnNewSageActions?.Invoke(this, actions);
        }

        public void JobExecutionVetoed(IJobExecutionContext context)
        {
        }

        public void JobToBeExecuted(IJobExecutionContext context)
        {
        }

    }
}