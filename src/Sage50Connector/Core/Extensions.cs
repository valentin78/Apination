using System;
using System.Reflection;

namespace Sage50Connector.Core
{
    static class Extensions
    {
        public static void LogLoaderExceptions(this ReflectionTypeLoadException exc, Action<Exception, Exception> log)
        {
            foreach (var le in exc.LoaderExceptions) log(exc, le);
        }
    }
}
