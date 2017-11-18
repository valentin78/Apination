using System;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sage50Connector.Processing.Actions.SageActions.Factory
{
    /// <summary>
    /// Creates Sage Actions from JSON strings
    /// </summary>
    class SageActionJsonFactory : ISageActionFactory
    {
        public static readonly ILog Log = LogManager.GetLogger(typeof(SageActionJsonFactory));

        public SageAction Create(string jsonString)
        {
            Log.DebugFormat("Creating Sage50 Action from JSON: {0}", jsonString);

            dynamic sageAction = JObject.Parse(jsonString);
            
            var actionType = GetActionTypeByPrefix((string)sageAction.type);

            return (SageAction)JsonConvert.DeserializeObject(jsonString, actionType);
        }

        private Type GetActionTypeByPrefix(string actionPrefix)
        {
            return Type.GetType($"Sage50Connector.Processing.Actions.SageActions.{actionPrefix}SageAction");
        }
    }
}