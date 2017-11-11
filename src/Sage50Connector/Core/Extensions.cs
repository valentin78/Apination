using System;
using System.Reflection;
using Quartz;

namespace Sage50Connector.Core
{
    /// <summary>
    /// Some helpful extensions
    /// </summary>
    static class Extensions
    {
        /// <summary>
        /// Writes all ReflectionTypeLoadException exception content to Log
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="log"></param>
        public static void LogLoaderExceptions(this ReflectionTypeLoadException ex, Action<Exception, Exception> log)
        {
            foreach (var le in ex.LoaderExceptions) log(ex, le);
        }

        public static TResult JobParam<TResult>(this IJobExecutionContext context, string key)
        {
            return (TResult)context.JobDetail.JobDataMap[key];
        }
    }
}
