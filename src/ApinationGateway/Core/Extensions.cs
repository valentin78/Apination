using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ApinationGateway.Core
{
    static class Extensions
    {
        public static void LogLoaderExceptions(this ReflectionTypeLoadException exc, Action<Exception, Exception> log)
        {
            foreach (var le in exc.LoaderExceptions) log(exc, le);
        }
    }
}
