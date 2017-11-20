using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using Newtonsoft.Json;
using Quartz;
using Quartz.Impl;
using Sage50Connector.API;
using Sage50Connector.Core;
using Sage50Connector.Models;
using Sage50Connector.Processing.Actions.ActionHandlers;
using Sage50Connector.Processing.Actions.ActionHandlers.Factory;
using Sage50Connector.Processing.Actions.SageActions;
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
                var apinationApi = new ApinationApi(new WebClientHttpUtility());
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
                scheduler.JobFactory = new PollApinationJobFactory(apinationApi);

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
                        List<PatchAction> patchList = new List<PatchAction>();
                        foreach (var action in actions)
                        {
                            try
                            {
                                Log.InfoFormat("Create handler for action (type: {0}, id: {1}) ...", action.type, action.id);
                                using (var handler = SageActionHandlerFactory.CreateHandler(action))
                                {
                                    Log.InfoFormat("Handling action (type: {0}, id: {1}) ...", action.type, action.id);
                                    // dynamic ActionHandler generic type require derived type, not base SageAction type
                                    var processed = handler.Handle((dynamic)action);
                                    Log.InfoFormat("Handling action result: {0}", processed);

                                    patchList.Add(new PatchAction() { Id = action.id, Processed = processed });
                                }
                            }
                            catch (Exception ex)
                            {
                                Log.Error("Handling action failed", ex);
                                patchList.Add(new PatchAction() { Id = action.id, Processed = false });
                            }
                        }

                        Log.InfoFormat("Sending actions Patch: {0}", JsonConvert.SerializeObject(patchList));

                        apinationApi.PatchActions(patchList);
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

