using System;
using System.Diagnostics;
using System.Linq;
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
    /// Provides operational interface for Apination Actions
    /// </summary>
    class SageActionsProcessor : IDisposable
    {
        public static readonly ILog Log = LogManager.GetLogger(typeof(SageActionsProcessor));

        private IScheduler scheduler;

        public SageActionsObserverable StartPollApination(Config config)
        {
            Log.InfoFormat("StartPollApination running with config: '{0}'", config);

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
            var apinationApi = new ApinationApi(new WebClientHttpUtility(), config);
            scheduler.JobFactory = new JobFactory(apinationApi);

            var apinationObservable = new SageActionsObserverable(
                job: pollApinationJob,
                trigger: cronTrigger,
                scheduler: scheduler,
                apinationJobListener: new PollApinationJobListener(new SageActionFromJsonFactory()),
                config: config
            );

            apinationObservable.Subscribe(sageActions =>
            {
                // actions can be handled in any order, this is the right place to put this logic
                // most of the time it will be just 1-1 action to handler assocciation
                try
                {
                    var actions = sageActions.ToArray();
                    //var patchList = new List<ProcessingStatus>();
                    foreach (var sageAction in actions)
                    {
                        var actionId = sageAction.triggerId;
                        try
                        {
                            Log.InfoFormat("Create handler for action (type: {0}, id: {1}) ...", sageAction.type, actionId);
                            using (var handler = SageActionHandlerFactory.CreateHandler(sageAction))
                            {
                                Log.InfoFormat("Handling action (type: {0}, id: {1}) ...", sageAction.type, actionId);
                                // dynamic ActionHandler generic type require derived type, not base SageAction type
                                handler.Handle((dynamic)sageAction);

                                Log.InfoFormat("Handling finnished success (type: {0}, id: {1}) ...", sageAction.type, actionId);
                                //Debugger.Break();
                                sageAction.ProcessingStatus = new ProcessingStatus
                                {
                                    Status = Status.SUCCESS
                                };
                            }
                        }
                        catch (MessageException ex)
                        {
                            Log.ErrorFormat("HANDLING ERROR MESSAGE: {0}", ex.Message);
                            sageAction.ProcessingStatus = new ProcessingStatus
                            {
                                Status = Status.FAIL,
                                Error = ex.Message
                            };
                        }
                        catch (Exception ex)
                        {
                            Log.Error("Handling action failed", ex);
                            sageAction.ProcessingStatus = new ProcessingStatus
                            {
                                Status = Status.FAIL,
                                Error = ex.Message
                            };
                        }
                    }

                    var settings = new JsonSerializerSettings
                    {
                        ContractResolver = new NoPayloadContractResolver()
                    };
                    var sageProcessingStatusJson = JsonConvert.SerializeObject(actions, settings);

                    Log.InfoFormat("Sending actions processing status: {0}", sageProcessingStatusJson);

                    apinationApi.PatchActions(sageProcessingStatusJson);
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

        public void Dispose()
        {
            scheduler?.Shutdown();
        }
    }
}

