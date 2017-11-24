using System;
using log4net;
using Quartz;
using Sage50Connector.API;

namespace Sage50Connector.Processing.HeartBeat
{
    /// <summary>
    /// HeartBeat Job
    /// </summary>
    class HeartBeatJob : IJob
    {
        public static readonly ILog Log = LogManager.GetLogger(typeof(HeartBeatJob));

        private readonly ApinationApi api;

        public HeartBeatJob(ApinationApi api)
        {
            this.api = api;
        }

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                Log.Info(" ------------------------------------------------------------------------------------------------------------------------------");
                Log.Info("| HeartBeatJob started ");
                Log.Info(" ------------------------------------------------------------------------------------------------------------------------------");

                Log.Info("Send handshake to Apination ...");
                api.Handshake();
                Log.Info("Success!");
            }
            catch (Exception ex)
            {
                Log.Error("Unknown exception was handled from Apination: ", ex);
            }
        }
    }
}