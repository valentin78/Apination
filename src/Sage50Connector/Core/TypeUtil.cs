using System;
using System.Linq;
using Sage50Connector.Models;
using Sage50Connector.Models.BindingTypes;
using Sage50Connector.Processing.Actions;
using Sage50Connector.Processing.Triggers;

namespace Sage50Connector.Core
{
    class TypeUtil
    {
        public static ISage50Trigger<TModel> ActivateTriggerByEventBindingType<TModel>(Sage50EventBindingTypes bindingType)
        {
            return ActivateInstanceByEventBindingType<ISage50Trigger<TModel>>((byte) bindingType);
        }
        
        public static IApinationAction<TModel> ActivateActionByEventBindingType<TModel>(ApinationEventBindingTypes bindingType)
        {
            return ActivateInstanceByEventBindingType<IApinationAction<TModel>>((byte)bindingType);
        }

        /// <summary>
        /// Activate Trigger|Action by event binding type param
        /// </summary>
        /// <param name="bindingType"></param>
        /// <returns></returns>
        private static T ActivateInstanceByEventBindingType<T>(byte bindingType)
        {
            var typesList = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(T).IsAssignableFrom(p));

            var processType = typesList.SingleOrDefault(p =>
            {
                var attrs = p.GetCustomAttributes(typeof(EventBindingAttribute), inherit: true);
                return attrs.Length != 0 && attrs.Select(attr => ((EventBindingAttribute) attr).Type).Any(type => type == bindingType);
            });

            return (T)Activator.CreateInstance(processType ?? throw new InvalidOperationException($"Can not find type by binding type {bindingType} and base type {typeof(T).Name}"));
        }
    }
}
