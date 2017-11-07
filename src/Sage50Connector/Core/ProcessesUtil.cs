﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Quartz;

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
        /// search IProcess type with Guid attribute value equal processId parameter
        /// </summary>
        /// <param name="processId"></param>
        /// <returns></returns>
        public static Type GetProcessTypeLocatorById(string processId)
        {
            var processType = _processTypes.SingleOrDefault(p =>
            {
                var attrs = p.GetCustomAttributes(typeof(GuidAttribute), inherit: true);
                return attrs.Length != 0 && attrs.Select(attr => ((GuidAttribute) attr).Value).Any(guid => guid == processId);
            });
            return processType;
        }
    }
}
