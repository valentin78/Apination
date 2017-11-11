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
    /// Observe Apination for data changes and activate appropriate actions for update Sage50 DB
    /// </summary>
    class ApinationObserver : ProcessBase
    {
        protected override void Process(IJobExecutionContext context)
        {
            var config = context.JobParam<Config>("Config");
            var apinationApi = context.JobParam<ApinationApi>("ApinationApi");

            throw new NotImplementedException();
        }
    }
}
