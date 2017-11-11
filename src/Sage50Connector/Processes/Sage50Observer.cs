using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Sage50Connector.API;
using Sage50Connector.Core;
using Sage50Connector.Models;

namespace Sage50Connector.Processes
{
    /// <summary>
    /// Observe Sage50 for data changes and activate appropriate triggers for DTO to Apination
    /// </summary>
    class Sage50Observer: ProcessBase
    {
        protected override void Process(IJobExecutionContext context)
        {
            var config = context.JobParam<Config>("Config");
            var sage50Api = context.JobParam<Sage50Api>("Sage50Api");

            throw new NotImplementedException();
        }
    }
}
