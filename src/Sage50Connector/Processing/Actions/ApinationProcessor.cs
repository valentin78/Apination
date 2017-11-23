using System;
using System.Collections.Generic;
using log4net;
using Newtonsoft.Json;
using Quartz;
using Quartz.Impl;
using Sage50Connector.API;
using Sage50Connector.Core;
using Sage50Connector.Models;
using Sage50Connector.Processing.Actions.ActionHandlers.Factory;
using Sage50Connector.Processing.Actions.SageActions;
using Sage50Connector.Processing.Actions.SageActions.Factory;

namespace Sage50Connector.Processing.Actions
{
    /// <summary>
    /// 
    /// </summary>
    class ApinationProcessor : IDisposable
    {
        public static readonly ILog Log = LogManager.GetLogger(typeof(ApinationProcessor));

        private IScheduler scheduler;

        public SageActionsObserverable StartPollApination(Config config)
        {
            Log.InfoFormat("StartPollApination running with config: '{0}'", config);

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
                scheduler.JobFactory = new JobFactory(apinationApi);

                // from the following observable definition it's immediately obvious that
                // it's composed from job, particular cron trigger, scheduler and listener
                SageActionsObserverable apinationObservable = new SageActionsObserverable(
                    job: pollApinationJob,
                    trigger: cronTrigger,
                    scheduler: scheduler,
                    apinationJobListener: new PollApinationJobListener(new SageActionFromJsonFactory()),
                    config: config
                );

                //and for apination
                apinationObservable.Subscribe(sageActions =>
                {
                    // actions can be handled in any order, this is the right place to put this logic
                    // most of the time it will be just 1-1 action to handler assocciation
                    try
                    {
                        var patchList = new List<SageActionPatch>();
                        foreach (var sageAction in sageActions)
                        {
                            try
                            {
                                Log.InfoFormat("Create handler for action (type: {0}, id: {1}) ...", sageAction.type, sageAction.id);
                                using (var handler = SageActionHandlerFactory.CreateHandler(sageAction))
                                {
                                    Log.InfoFormat("Handling action (type: {0}, id: {1}) ...", sageAction.type, sageAction.id);
                                    // dynamic ActionHandler generic type require derived type, not base SageAction type
                                    var processed = handler.Handle((dynamic)sageAction);
                                    Log.InfoFormat("Handling action result: {0}", processed);

                                    patchList.Add(new SageActionPatch { Id = sageAction.id, Processed = processed });
                                }
                            }
                            catch (MessageException ex)
                            {
                                Log.ErrorFormat("HANDLING ERROR MESSAGE: {0}", ex.Message);
                                patchList.Add(new SageActionPatch { Id = sageAction.id, Processed = false });
                            }
                            catch (Exception ex)
                            {
                                Log.Error("Handling action failed", ex);
                                patchList.Add(new SageActionPatch { Id = sageAction.id, Processed = false });
                            }
                        }

                        Log.InfoFormat("Sending actions Patch: {0}", JsonConvert.SerializeObject(patchList));

                        apinationApi.PatchActions(patchList);
                    }
                    catch (MessageException ex)
                    {
                        Log.ErrorFormat("HANDLING ERROR MESSAGE: {0}", ex.Message);
                    }
                    catch (Exception ex)
                    {
                        Log.Error("ACTIONS PROCESSING ERROR: ", ex);
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

