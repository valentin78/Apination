using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Quartz;

namespace Sage50Connector.Core
{
    class Helpers
    {
        private static readonly IEnumerable<Type> _processTypes;

        static Helpers()
        {
            // cache assembly types that inherit ProcessBase & IJob
            _processTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(ProcessBase).IsAssignableFrom(p))
                .Where(p => typeof(IJob).IsAssignableFrom(p));
        }

        /// <summary>
        /// search IProcess type with Guid attribute value equal processID parameter
        /// </summary>
        /// <param name="processID"></param>
        /// <returns></returns>
        public static Type ProcessTypeLocator(string processID)
        {
            var processType = _processTypes.SingleOrDefault(p =>
            {
                var attrs = p.GetCustomAttributes(typeof(GuidAttribute), true);
                return attrs.Length != 0 && attrs.Select(attr => ((GuidAttribute) attr).Value).Any(guid => guid == processID);
            });
            return processType;
        }
    }
}
