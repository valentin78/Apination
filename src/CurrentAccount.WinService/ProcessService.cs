using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
using System.ServiceModel;
using System.Threading;
using CurrentAccount.Data.Config;
using CurrentAccount.Data.Models;
using CurrentAccount.Data.Models.ContUnic;
using CurrentAccount.Data.Models.ContUnic.Enums;
using CurrentAccount.Data.Processes;
using CurrentAccount.Data.Repositories.DB;
using Microsoft.Reporting.WebForms;
using log4net;
using log4net.Config;

namespace CurrentAccount.WinService
{
    /// <summary>
    /// Диспетчер процессов
    /// </summary>
    [Synchronization]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single, InstanceContextMode = InstanceContextMode.Single)]
    [SvcErrorHandlerBehaviour]
    public class ProcessService : IProcessService
    {
        #region Logger

        // ReSharper disable InconsistentNaming
        private static readonly ILog log = LogManager.GetLogger(typeof(ProcessService));
        // ReSharper restore InconsistentNaming

        /// <summary>
        /// Логер сервиса
        /// </summary>
        public static ILog Log { get { return log; } }

        /// <summary>
        /// инициализация логера
        /// </summary>
        public static void InitializeLogger() { XmlConfigurator.Configure(); }

        #endregion

        static ProcessService()
        {
            InitializeLogger();
        }

       
        /// <summary>
        /// Конструктор
        /// </summary>
        public ProcessService()
        {
            ProcessList = new List<ProcessBase>();

            try
            {
                (new ProcedureRepository()).ResetStatuses();
            }
            catch (ReflectionTypeLoadException ex)
            {
                var typeLoadException = ex;
                var loaderExceptions = typeLoadException.LoaderExceptions;
                foreach (var item in loaderExceptions)
                {
                    Log.Error(item);
                }
                throw;
            }
            SchedulerThread = new Thread(Scheduler);
            SchedulerThread.Start();
        }

        private void Scheduler()
        {
            try
            {
                Log.Info("Запуск планировщика процессов...");
            }
            catch (ThreadAbortException)
            {
                Log.Info("Планировщик процессов остановлен");
            }
            catch (Exception exc)
            {
                Log.FatalFormat("Ошибка в планировщике процессов: {0}. Trace: {1}", exc.Message, exc);
            }
        }

    }
}
