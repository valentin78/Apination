using System;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sage50Connector.Processing.Actions.SageActions.Factory
{
    /// <summary>
    /// Creates Sage Actions from JSON strings
    /// </summary>
    public class SageActionFromJsonFactory : ISageActionFactory
    {
        public static readonly ILog Log = LogManager.GetLogger(typeof(SageActionFromJsonFactory));

        public SageAction Create(string jsonString)
        {
            Log.DebugFormat("Creating Sage50 Action from JSON: {0}", jsonString);

            dynamic sageAction = JObject.Parse(jsonString);

            var actionTypePrefix = (string)sageAction.type;
            var actionType = SageAction.GetActionClassType(actionTypePrefix);
            if (actionType == null)
            {
                throw new Exception($"Can not found action type for '{actionTypePrefix}' type");
            }

            return (SageAction)JsonConvert.DeserializeObject(jsonString, actionType);
        }

    }
}