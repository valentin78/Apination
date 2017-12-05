using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using Sage50Connector.Processing.Actions.SageActions;

namespace Sage50Connector.Processing.Actions.ActionHandlers.Factory
{
    public class SageActionHandlerFactory
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SageActionHandlerFactory));
        private static readonly Dictionary<Type, object> HandlersCache = new Dictionary<Type, object>();

        /// <summary>
        /// Create Action handler by action type. 
        /// Uses dynamic as result because generic interface ISageActionHandler uses contravariant type and cannot cast to generic by base type 
        /// </summary>
        public static dynamic CreateHandler(string actionType)
        {
            Log.DebugFormat("Creating ISageActionHandler for received SageAction ...");

            var actionHandlerType = typeof(ISageActionHandler<>);
            var classType = SageAction.GetActionClassType(actionType);

            if (HandlersCache.ContainsKey(classType))
            {
                return HandlersCache[classType];
            }

            var actionHandlerTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => Any(type, actionHandlerType, classType)).ToArray();

            if (actionHandlerTypes.Length != 1)
                throw new Exception($"Not found or more than one action handlers implememntations for action type: {actionType}");
            var handler = Activator.CreateInstance(actionHandlerTypes[0]);
            HandlersCache.Add(classType, handler);
            return handler;
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