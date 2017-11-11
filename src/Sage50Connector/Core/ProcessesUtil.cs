using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Quartz;
using Sage50Connector.Models;
using Sage50Connector.Processes;

namespace Sage50Connector.Core
{
    class ProcessesUtil
    {
        private static readonly IEnumerable<Type> _processTypes;

        static ProcessesUtil()
        {
            // cache assembly types that inherit ProcessBase & IJob
            _processTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(ProcessBase).IsAssignableFrom(p))
                .Where(p => typeof(IJob).IsAssignableFrom(p));
        }

        /// <summary>
        /// Activate Trigger|Action by event binding type param
        /// </summary>
        /// <param name="bindingType"></param>
        /// <returns></returns>
        public static T ActivateByEventBinding<T>(EventBindingTypes bindingType)
        {
            var processType = _processTypes.SingleOrDefault(p =>
            {
                var attrs = p.GetCustomAttributes(typeof(T), inherit: true);
                return attrs.Length != 0 && attrs.Select(attr => ((EventBindingAttribute) attr).Type).Any(type => type == bindingType);
            });

            return (T)Activator.CreateInstance(processType ?? throw new InvalidOperationException($"Can not find type by binding type {bindingType} and base type {typeof(T).Name}"));
        }
    }
}
