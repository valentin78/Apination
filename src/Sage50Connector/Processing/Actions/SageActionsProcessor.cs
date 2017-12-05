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
        private static readonly ILog Log = LogManager.GetLogger(typeof(SageActionsProcessor));

        private IScheduler scheduler;

        public SageActionsObserverable StartActionsProcessing(Config config)
        {
            var apinationApi = new ApinationApi(new WebClientHttpUtility(), config);
            var observable = StartPollApination(config, apinationApi);

            observable.Subscribe(sageActions => ProcessSageActions(sageActions, apinationApi));
            return observable;
        }

        private static void ProcessSageActions(IEnumerable<SageAction> sageActions, ApinationApi apinationApi)
        {
            // actions can be handled in any order, this is the right place to put this logic
            // most of the time it will be just 1-1 action to handler assocciation

            var actions = sageActions.ToArray();
            foreach (var sageAction in actions)
            {
                try
                {
                    Log.InfoFormat("Create handler for action (type: {0}, id: {1}) ...", sageAction.type,
                        sageAction.id);
                    using (var handler = SageActionHandlerFactory.CreateHandler(sageAction.type))
                    {
                        Log.InfoFormat("Handling action (type: {0}, id: {1}) ...", sageAction.type, sageAction.id);
                        // dynamic ActionHandler generic type require derived type, not base SageAction type
                        handler.Handle((dynamic)sageAction);
                        Log.InfoFormat("Handling action successful (type: {0}, id: {1}) ...", sageAction.type, sageAction.id);

                        sageAction.ProcessingStatus = new ProcessingStatus
                        {
                            id = sageAction.id,
                            processingStatus = Status.SUCCESS
                        };

                        // sending Success log to apination ....
                        apinationApi.Log(new ApinationLogRecord
                        {
                            Status = "Success",
                            TriggerId = sageAction.triggerId,
                            Uid = sageAction.mainLogId,
                            Data = "{}",
                            Date = DateTime.Now
                        });
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Handling action failed", ex);
                    sageAction.ProcessingStatus = new ProcessingStatus
                    {
                        id = sageAction.id,
                        processingStatus = Status.FAIL,
                        error = ex.Message
                    };

                    // sending ex log to apination ....
                    var exJson = JsonConvert.SerializeObject(ex);
                    apinationApi.Log(new ApinationLogRecord
                    {
                        Message = ex.Message,
                        Status = "Fail",
                        TriggerId = sageAction.triggerId,
                        Uid = sageAction.mainLogId,
                        Data = exJson,
                        Date = DateTime.Now
                    });
                }
            }

            var sageProcessingStatusJson = JsonConvert.SerializeObject(actions.Select(i => i.ProcessingStatus));
            Log.InfoFormat("Sending actions processing status: {0}", sageProcessingStatusJson);
            var responce = apinationApi.ReportProcessingStatus(sageProcessingStatusJson);
            Log.InfoFormat("Actions processing status sent succesful: {0}", responce);
        }

        private SageActionsObserverable StartPollApination(Config config, ApinationApi apinationApi)
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

            scheduler.JobFactory = new PollApinationJobFactory(apinationApi);

            var sageActionsObservable = new SageActionsObserverable(
                job: pollApinationJob,
                trigger: cronTrigger,
                scheduler: scheduler,
                apinationJobListener: new PollApinationJobListener(new SageActionFromJsonFactory()),
                config: config
            );

            scheduler.Start();

            return sageActionsObservable;
        }

        public void Dispose()
        {
            scheduler?.Shutdown();
        }
    }
}

