using System;
using System.Linq;
using log4net;
using Sage50Connector.Processing.Actions.SageActions;

namespace Sage50Connector.Processing.Actions.ActionHandlers.Factory
{
    public class SageActionHandlerFactory
    {
        public static readonly ILog Log = LogManager.GetLogger(typeof(SageActionHandlerFactory));

        /// <summary>
        /// Create Action handler by action data. 
        /// (Type only currently used)
        /// Uses dynamic as result because generic interface ISageActionHandler uses contravariant type and cannot cast to generic by base type 
        /// </summary>
        public static dynamic CreateHandler(SageAction action)
        {
            Log.DebugFormat("Creating ISageActionHandler for received SageAction ...");

            var actionHandlerType = typeof(ISageActionHandler<>);
            var actionType = SageAction.GetActionClassType(action.type);

            var actionHandlerTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => Any(type, actionHandlerType, actionType)).ToArray();

            if (actionHandlerTypes.Length != 1)
                throw new Exception($"Not found or more than one action handlers implememntations for action type: {action.type}");

            return Activator.CreateInstance(actionHandlerTypes[0]);
        }

        /// <summary>
        /// Any of three types comparer
        /// </summary>
        private static bool Any(Type type, Type actionHandlerType, Type actionType)
        {
            var implementActionHandlers = type.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == actionHandlerType);
            return implementActionHandlers.Any(@interface => @interface.GenericTypeArguments.Length == 1 && @interface.GenericTypeArguments[0] == actionType);
        }
    }


}