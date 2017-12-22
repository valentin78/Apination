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

        public event EventHandler<Exception> OnSageActionError;

        public void JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException)
        {
            if (context.JobDetail.JobDataMap["actions"] == null) return;

            var sageActionsJson = (IEnumerable<string>)context.JobDetail.JobDataMap["actions"];
            
            Log.Debug("Creating Sage50 actions from JSON ...");
            try
            {
                var sageActions = sageActionsJson.Select(actionJson => actionFactory.Create(actionJson));

                NewSageActions(sageActions);
            }
            catch (Exception ex)
            {
                SageActionError(ex);
            }
        }

        protected virtual void NewSageActions(IEnumerable<SageAction> actions)
        {
            Log.Debug("Trigger NewSageActions event ...");
            OnNewSageActions?.Invoke(this, actions);
        }

        protected virtual void SageActionError(Exception ex)
        {
            Log.Debug("Trigger SageActionError event ...");
            OnSageActionError?.Invoke(this, ex);
        }

        public void JobExecutionVetoed(IJobExecutionContext context)
        {
        }

        public void JobToBeExecuted(IJobExecutionContext context)
        {
        }

    }
}