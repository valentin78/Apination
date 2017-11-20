using System;
using System.Linq;

namespace Sage50Connector.Core
{
    class TypeUtil
    {
        /// <summary>
        /// Activate Trigger|Action by event binding type param
        /// </summary>
        /// <param name="bindingType"></param>
        /// <returns></returns>
        private static T CreateInstanceByEventBindingType<T>(byte bindingType)
        {
            var typesList = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(T).IsAssignableFrom(p));

            var processType = typesList.SingleOrDefault(p =>
            {
                var attrs = p.GetCustomAttributes(typeof(EventBindingAttribute), inherit: true);
                return attrs.Length != 0 && attrs.Select(attr => ((EventBindingAttribute) attr).Type).Any(type => type == bindingType);
            });

            if (processType == null) throw new InvalidOperationException($"Can not find type by binding type {bindingType} and base type {typeof(T).Name}");

            return (T)Activator.CreateInstance(processType);
        }

        /// <summary>
        /// Convert datetime to UTC string format
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string DateToUTC(DateTime value)
        {
            return value.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffK");
        }


        /// <summary>
        /// Convert datetime to ODBC string format
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string DateToODBC(DateTime value)
        {
            return value.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
