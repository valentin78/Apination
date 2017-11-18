using System;
using System.IO;
using log4net;
using Newtonsoft.Json;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using Sage50Connector.Models;

namespace Sage50Connector.Architecture
{
    // Fabric Types

    internal interface IObserverFabrik
    {
        IObserver Create(Config config);
    }

    class ApinationObserverFabrik : IObserverFabrik
    {
        public static readonly ILog Log = LogManager.GetLogger(typeof(ApinationObserverFabrik));

        public IObserver Create(Config config)
        {
            Log.Info("Create CronObserver for ApinationObserver ...");

            var apinationObserver = new CronObserver(config.ApinationCronSchedule);
            
            apinationObserver.AddChecker(new ApinationChecker(config));
            
            apinationObserver.Subscribe(new CreateCustomerToSage50());
            apinationObserver.Subscribe(new CreateInvoiceToSage50());

            return apinationObserver;
        }
    }

    class Sage50ObserverFabrik : IObserverFabrik
    {
        public static readonly ILog Log = LogManager.GetLogger(typeof(Sage50ObserverFabrik));

        public IObserver Create(Config config)
        {
            Log.Info("Create CronObserver for Sage50ObserverFabrik ...");

            var sage50Observer = new CronObserver(config.Sage50CronSchedule);
            
            sage50Observer.AddChecker(new Sage50CustomerChecker(config));
            sage50Observer.AddChecker(new Sage50InvoiceChecker(config));
            
            sage50Observer.Subscribe(new UpdateCustomersToApination(config));
            sage50Observer.Subscribe(new UpdateInvoicesToApination(config));

            return sage50Observer;
        }
    }

    class EventData
    {
        public string Type { get; set; }
        public object Payload { get; set; }

        public override string ToString()
        {
            using (var writer = new StringWriter())
            {
                JsonSerializer.Create().Serialize(writer, this);
                return writer.ToString();
            }
        }
    }

    class CronScheduleFabrik: IDisposable
    {
        public static readonly ILog Log = LogManager.GetLogger(typeof(CronScheduleFabrik));

        // ReSharper disable once ClassNeverInstantiated.Local
        [DisallowConcurrentExecution]
        private class ScheduledProcess : IJob
        {
            public void Execute(IJobExecutionContext context)
            {
            }
        }

        public class JobListener : IJobListener
        {
            public static readonly ILog Log = LogManager.GetLogger(typeof(CronScheduleFabrik));

            /// <summary>
            /// technical jobName for debuggind & logging purposes
            /// </summary>
            public string JobIdentityName { get; private set; }

            public JobListener(string jobIdentityName)
            {
                JobIdentityName = jobIdentityName;
            }

            public event EventHandler OnExecuted;

            public void JobToBeExecuted(IJobExecutionContext context) { }

            public void JobExecutionVetoed(IJobExecutionContext context) { }

            public void JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException)
            {
                Log.InfoFormat("JobWasExecuted; JobIdentityName: {0}", JobIdentityName);
                OnExecuted?.Invoke(this, EventArgs.Empty);
            }

            public string name = Guid.NewGuid().ToString();
            public string Name => name;
        }

        private IScheduler scheduler;
        private readonly string JobIdentityName = Guid.NewGuid().ToString();

        public JobListener Create(string cronPeriod)
        {
            Log.InfoFormat("Statring Scheduler for cronPeriod: {0}; JobIdentityName: {1}", cronPeriod, JobIdentityName);

            var schedulerFactory = new StdSchedulerFactory();
            scheduler = schedulerFactory.GetScheduler();

            scheduler.Start();
            
            var job = JobBuilder.Create<ScheduledProcess>()
                .WithIdentity(JobIdentityName)
                .Build();

            var trigger = TriggerBuilder.Create().WithIdentity(JobIdentityName)
                .StartNow()
                .WithCronSchedule(cronPeriod)
                .Build();

            scheduler.ScheduleJob(job, trigger);

            var jobListener = new JobListener(JobIdentityName);

            scheduler.ListenerManager.AddJobListener(jobListener, KeyMatcher<JobKey>.KeyEquals(new JobKey(JobIdentityName)));
            return jobListener;
        }

        public void Dispose()
        {
            scheduler.Shutdown(waitForJobsToComplete: true);
        }
    }

    class CronObserver : IObserver
    {
        public static readonly ILog Log = LogManager.GetLogger(typeof(CronObserver));

        // ReSharper disable once InconsistentNaming
        private readonly CronScheduleFabrik.JobListener listener;
        
        private readonly CronScheduleFabrik cronScheduleFabrik;

        public string Identity => listener.JobIdentityName;

        public CronObserver(string cronPeriod)
        {
            cronScheduleFabrik = new CronScheduleFabrik();
            listener = cronScheduleFabrik.Create(cronPeriod);
        }

        public delegate void DataHandler(EventData data, EventArgs e);
        private event DataHandler OnDataEvent;

        public void TriggerOnDataEvent(EventData data)
        {
            OnDataEvent?.Invoke(data, EventArgs.Empty);
        }

        public void AddChecker(IChecker checker)
        {
            Log.Info("=> Register checker: " + listener.JobIdentityName);

            var o = this;
            listener.OnExecuted += (sender, args) => {
                try
                {
                    checker.Check(o);
                }
                catch (Exception exc)
                {
                    Log.Debug("!CHECKING ERROR: {0}", exc);
                }
            };
        }

        public void Subscribe(ISaver saver)
        {
            OnDataEvent += (data, args) =>
            {
                if (data.Type == saver.PayloadTypeFiler)
                {
                    saver.Save(data);
                    //Log.InfoFormat("OnDataEvent raised with Data: {0};", data);
                }
            };
        }

        public void Dispose()
        {
            cronScheduleFabrik.Dispose();
        }
    }

    internal interface IObserver: IDisposable
    {
        string Identity { get;  }

        void TriggerOnDataEvent(EventData data);
        void AddChecker(IChecker workflow);
        void Subscribe(ISaver saver);
    }

    interface IChecker
    {
        void Check(IObserver o);
    }

    interface ISaver
    {
        string PayloadTypeFiler { get; }
        void Save(EventData data);
    }

    // Apination -> Sage50
    class ApinationChecker : IChecker
    {
        public static readonly ILog Log = LogManager.GetLogger(typeof(ApinationChecker));


        private readonly Config _config;

        public ApinationChecker(Config config)
        {
            _config = config;
        }

        void IChecker.Check(IObserver o)
        {
            Log.InfoFormat("Checking from Apination Service Data changes '{0}' ...", o.Identity);

            var url = _config.ApinationActionEndpointUrl;

            // get data from Apination Endpoint
            // parsing responce 

            // customers first order
            o.TriggerOnDataEvent(new EventData
            {
                Type = "CreateCustomer",
                Payload = new object()
            });

            // invoices second, after customers
            o.TriggerOnDataEvent(new EventData
            {
                Type = "CreateInvoice",
                Payload = new object()
            });
        }

    }

    class CreateCustomerToSage50 : ISaver
    {
        public static readonly ILog Log = LogManager.GetLogger(typeof(CreateCustomerToSage50));


        public string PayloadTypeFiler => "CreateCustomer";

        public void Save(EventData data)
        {
            Log.InfoFormat("Save Customer to Sage50: {0}", data);

            // save to sage50
        }
    }

    class CreateInvoiceToSage50 : ISaver
    {
        public static readonly ILog Log = LogManager.GetLogger(typeof(CreateInvoiceToSage50));

        public string PayloadTypeFiler => "CreateInvoice";

        public void Save(EventData data)
        {
            Log.InfoFormat("Save Invoice to Sage50: {0}", data);
        }
    }

    // Sage50 -> Apination

    class Sage50CustomerChecker : IChecker
    {
        public static readonly ILog Log = LogManager.GetLogger(typeof(ApinationChecker));

        private readonly Config _config;

        public Sage50CustomerChecker(Config config)
        {
            _config = config;
        }
        public void Check(IObserver o)
        {
            Log.InfoFormat("Checking from Sage50 Customers List changes '{0}' ...", o.Identity);

            //var companiesList = _config.CompaniesList;
            // foreach companies check customers updates

            // for customers 
            o.TriggerOnDataEvent(new EventData
            {
                Type = "UpdateCustomers",
                Payload = new object() // customersList, companyId
            });
        }
    }

    class Sage50InvoiceChecker : IChecker
    {
        public static readonly ILog Log = LogManager.GetLogger(typeof(Sage50InvoiceChecker));

        private readonly Config _config;

        public Sage50InvoiceChecker(Config config)
        {
            _config = config;
        }
        public void Check(IObserver o)
        {
            Log.InfoFormat("Checking from Sage50 Invoices List changes '{0}' ...", o.Identity);

            //var companiesList = _config.CompaniesList;
            // foreach companies check invoices updates

            // for invoices 
            o.TriggerOnDataEvent(new EventData
            {
                Type = "UpdateInvoices",
                Payload = new object() // invoicesList, companyId
            });
        }
    }

    class UpdateCustomersToApination : ISaver
    {
        public static readonly ILog Log = LogManager.GetLogger(typeof(UpdateCustomersToApination));

        private Config _config;

        public UpdateCustomersToApination(Config config)
        {
            _config = config;
        }
        public string PayloadTypeFiler => "UpdateCustomers";

        public void Save(EventData data)
        {
            Log.InfoFormat("POST Customers to Apination: {0}", data);

            // url for UpdateCustomers
            //var url = _config.TriggersConfig[PayloadTypeFiler].ApinationEndpointUrl;
            // send customers to Apination URL
        }
    }

    class UpdateInvoicesToApination : ISaver
    {
        public static readonly ILog Log = LogManager.GetLogger(typeof(UpdateInvoicesToApination));

        private Config _config;

        public UpdateInvoicesToApination(Config config)
        {
            _config = config;
        }
        public string PayloadTypeFiler => "UpdateInvoices";

        public void Save(EventData data)
        {
            Log.InfoFormat("POST Invoices to Apination: {0}", data);

            // url for UpdateCustomers
            //var url = _config.TriggersConfig[PayloadTypeFiler].ApinationEndpointUrl;
            // send invoices to Apination URL
        }
    }

    // customer, invoice, payment
}
