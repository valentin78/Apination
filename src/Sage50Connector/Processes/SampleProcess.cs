using System.Diagnostics;
using System.Runtime.InteropServices;
using Quartz;
using Sage50Connector.Core;
using Sage50Connector.Models;
using Sage50Connector.Repositories;

namespace Sage50Connector.Processes
{
    [Guid("CBD51F9F-4B8D-40A2-B086-1F849894EB96")]
    class SampleProcess : ProcessBase
    {
        protected override void Process(IJobExecutionContext context)
        {
            base.Process(context);

            var p1 = context.JobDetail.JobDataMap["p1"];
            var p2 = context.JobDetail.JobDataMap["p2"];
            Log.InfoFormat("-> SampleProcess started ... p1: {0}; p2: {1}", p1, p2);
            
            using (var sage50Api = new Sage50Repository())
            {
                var company = context.JobDetail.JobDataMap["$company"] as Company;
                var id = sage50Api.OpenCompany(company.CompanyName);
                Log.InfoFormat("-> Company open success: {0}", id.CompanyName);

            }
        }
    }
}
