using System;
using System.Reflection;
using log4net;
using Sage50Connector.Processing.Actions.SageActions;

namespace Sage50Connector.Processing.Actions.ActionHandlers.Factory
{
    public class SageActionHandlerFactory
    {
        public static readonly ILog Log = LogManager.GetLogger(typeof(SageActionHandlerFactory));

        public static ISageActionHandler CreateHandler(SageAction action)
        {
            Log.DebugFormat("Creating ISageActionHandler for received SageAction ...");

            var type = GetHandlerTypeByPrefix(action.type);
            if (type == null)
            {
                throw new Exception($"Can not found handler type for '{action.type}' action");
            }

            return Activator.CreateInstance(type) as ISageActionHandler;
        }


        private static Type GetHandlerTypeByPrefix(string handlerPrefix)
        {
            return Type.GetType($"Sage50Connector.Processing.Actions.ActionHandlers.{handlerPrefix}SageActionHandler");
        }
    }
}