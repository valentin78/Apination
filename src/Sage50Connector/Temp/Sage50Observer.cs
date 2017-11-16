using System;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using Sage50Connector.Models;

namespace Sage50Connector.Temp
{

    // Fabric Types

    internal interface IObserverFabrik
    {
        IObserver Create(Config config);
    }

    class ApinationObserverFabrik : IObserverFabrik
    {
        public IObserver Create(Config config)
        {
            var apinationObserver = new CronObserver(config.ApinationCronSchedule);
            apinationObserver.AddChecker(new ApinationChecker(config));
            apinationObserver.Subscribe(new CreateCustomerToSage50());
            apinationObserver.Subscribe(new CreateInvoiceToSage50());

            return apinationObserver;
        }
    }

    class Sage50ObserverFabrik : IObserverFabrik
    {
        public IObserver Create(Config config)
        {
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
    }

    class CronScheduleFabrik
    {
        private class ScheduledProcess : IJob
        {
            public void Execute(IJobExecutionContext context)
            {
            }
        }

        public class JobListener : IJobListener
        {
            public event EventHandler OnExecuted;

            public void JobToBeExecuted(IJobExecutionContext context) { }

            public void JobExecutionVetoed(IJobExecutionContext context) { }

            public void JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException)
            {
                OnExecuted?.Invoke(this, EventArgs.Empty);
            }

            public string Name => "";
        }

        readonly Lazy<IScheduler> _scheduler = new Lazy<IScheduler>(() =>
        {
            var schedulerFactory = new StdSchedulerFactory();
            return schedulerFactory.GetScheduler();
        }, true);

        private IScheduler Scheduler => _scheduler.Value;

        public JobListener Create(string cronPeriod)
        {
            Scheduler.Start();

            var job = JobBuilder.Create<ScheduledProcess>().WithIdentity("jobName").Build();

            var trigger = TriggerBuilder.Create()
                .StartNow()
                .WithCronSchedule(cronPeriod)
                .Build();

            Scheduler.ScheduleJob(job, trigger);

            var jobListener = new JobListener();

            Scheduler.ListenerManager.AddJobListener(jobListener, KeyMatcher<JobKey>.KeyEquals(new JobKey("jobName")));
            return jobListener;
        }
    }

    class CronObserver : IObserver
    {
        // ReSharper disable once InconsistentNaming
        private CronScheduleFabrik.JobListener listener;

        public CronObserver(string cronPeriod)
        {
            listener = new CronScheduleFabrik().Create(cronPeriod);
        }

        public delegate void DataHandler(EventData data, EventArgs e);
        private event DataHandler OnDataEvent;

        public void TriggerOnDataEvent(EventData data)
        {
            OnDataEvent?.Invoke(null, EventArgs.Empty);
        }

        public void AddChecker(IChecker workflow)
        {
            listener.OnExecuted += (sender, args) => {
                workflow.Check(this);
            };
        }

        public void Subscribe(ISaver saver)
        {
            OnDataEvent += (data, args) =>
            {
                if (data.Type == saver.PayloadTypeFiler) saver.Save(data);
            };
        }
    }

    internal interface IObserver
    {
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
        private readonly Config _config;

        public ApinationChecker(Config config)
        {
            _config = config;
        }

        void IChecker.Check(IObserver o)
        {
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
        public string PayloadTypeFiler => "CreateCustomer";

        public void Save(EventData data)
        {
            {
                // save to sage50
            }
        }
    }

    class CreateInvoiceToSage50 : ISaver
    {
        public string PayloadTypeFiler => "CreateInvoice";

        public void Save(EventData data)
        {
            // save to sage50
        }
    }

    // Sage50 -> Apination

    class Sage50CustomerChecker : IChecker
    {
        private readonly Config _config;

        public Sage50CustomerChecker(Config config)
        {
            _config = config;
        }
        public void Check(IObserver o)
        {
            var companiesList = _config.CompaniesList;
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
        private readonly Config _config;

        public Sage50InvoiceChecker(Config config)
        {
            _config = config;
        }
        public void Check(IObserver o)
        {
            var companiesList = _config.CompaniesList;
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
        private Config _config;

        public UpdateCustomersToApination(Config config)
        {
            _config = config;
        }
        public string PayloadTypeFiler => "UpdateCustomers";

        public void Save(EventData data)
        {
            // url for UpdateCustomers
            var url = _config.TriggersConfig[PayloadTypeFiler].ApinationEndpointUrl;
            // send customers to Apination URL
        }
    }

    class UpdateInvoicesToApination : ISaver
    {
        private Config _config;

        public UpdateInvoicesToApination(Config config)
        {
            _config = config;
        }
        public string PayloadTypeFiler => "UpdateInvoices";

        public void Save(EventData data)
        {
            // url for UpdateCustomers
            var url = _config.TriggersConfig[PayloadTypeFiler].ApinationEndpointUrl;
            // send invoices to Apination URL
        }
    }

    // customer, invoice, payment
}
