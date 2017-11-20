using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using log4net;
using Sage50Connector.Processing.Actions.SageActions;
using Sage50Connector.Processing.Actions.SageActions.Factory;

namespace Sage50Connector.Processing.Actions.ActionHandlers.Factory
{
    public class SageActionHandlerFactory
    {
        public static readonly ILog Log = LogManager.GetLogger(typeof(SageActionHandlerFactory));

        /// <summary>
        /// Create Action handler by action type. Uses dynamic ad result because generic interface ISageActionHandler uses contravariant type and cannot cast to generic by base type 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static dynamic CreateHandler(SageAction action)
        {
            Log.DebugFormat("Creating ISageActionHandler for received SageAction ...");

            //var type = GetHandlerTypeByPrefix(action.type);
            //if (type == null)
            //{
            //    throw new Exception($"Can not found handler type for '{action.type}' action");
            //}

            var actionHandlerType = typeof(ISageActionHandler<>);
            var actionType = SageActionJsonFactory.GetActionTypeByPrefix(action.type);

            var actionHandlerTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => Any(type, actionHandlerType, actionType)).ToArray();

            if (actionHandlerTypes.Length != 1)
                throw new Exception($"Not found or more than one action handlers implememntations for action type: {action.type}");

            return Activator.CreateInstance(actionHandlerTypes[0]);
        }

        private static bool Any(Type type, Type actionHandlerType, Type actionType)
        {
            var implementActionHandlers = type.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == actionHandlerType);
            return implementActionHandlers.Any(@interface => @interface.GenericTypeArguments.Length == 1 && @interface.GenericTypeArguments[0] == actionType);
        }


        private static Type GetHandlerTypeByPrefix(string handlerPrefix)
        {
            return Type.GetType($"Sage50Connector.Processing.Actions.ActionHandlers.{handlerPrefix}SageActionHandler");
        }
    }


}