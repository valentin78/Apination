﻿using System;
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
        /// <summary>
        /// Activate Trigger|Action by event binding type param
        /// </summary>
        /// <param name="bindingType"></param>
        /// <returns></returns>
        public static T ActivateByEventBinding<T>(EventBindingTypes bindingType)
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
