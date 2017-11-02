using System;
using System.Reflection;

namespace Sage50Connector.Core
{
    static class Extensions
    {
        public static void LogLoaderExceptions(this ReflectionTypeLoadException ex, Action<Exception, Exception> log)
        {
            foreach (var le in ex.LoaderExceptions) log(ex, le);
        }
    }
}
