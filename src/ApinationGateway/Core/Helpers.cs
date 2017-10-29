using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApinationGateway.Processes;

namespace ApinationGateway.Core
{
    class Helpers
    {
        public static Type ProcessTypeLocator(string processID)
        {
            return typeof(SampleProcess);
        }
    }
}
