using System;
using System.Linq;
using log4net;
using Newtonsoft.Json;
using Quartz;
using Sage50Connector.API;

namespace Sage50Connector.Processing.Actions
{
    /// <summary>
    /// Polls Apination endpoint to check if new actions appeared
    /// </summary>
    class PollApinationJob : IJob
    {
        public static readonly ILog Log = LogManager.GetLogger(typeof(PollApinationJob));

        private readonly ApinationApi api;

        public PollApinationJob(ApinationApi api)
        {
            this.api = api;
        }

        public void Execute(IJobExecutionContext context)
        {
            Log.Info(" --------------------------");
            Log.Info("| PollApinationJob started |");
            Log.Info(" --------------------------");
            
            Log.Info("Get ActionsJson from Apination...");
            var jsonString = api.GetActionsJson();
            Log.DebugFormat("Received JSON: '{0}'", jsonString);

            // Splits actions to array and put them in context.JobDetail.JobDataMap
            dynamic actionsDynamic = JsonConvert.DeserializeObject(jsonString);
            var actionsStrings = (actionsDynamic.Root as Newtonsoft.Json.Linq.JArray)?.Select(i => i.ToString());

            context.JobDetail.JobDataMap.Add("actions", actionsStrings);
        }
    }
}