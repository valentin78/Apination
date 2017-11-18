using System;
using log4net;
using Quartz;
using Sage50Connector.API;
using Sage50Connector.Core;
using Sage50Connector.Models;

namespace Sage50Connector.Processing
{
    abstract class BaseObserver: IJob
    {
        public static readonly ILog Log = LogManager.GetLogger(typeof(Sage50Observer));
        
        protected Lazy<Sage50Api> Sage50Api => new Lazy<Sage50Api>(() => new Sage50Api());

        protected Lazy<ApinationApi> ApinationApi => new Lazy<ApinationApi>(() => new ApinationApi(new WebClientHttpUtility()));

        public Config Config { protected get; set; }

        public abstract void Execute(IJobExecutionContext context);
    }
}
