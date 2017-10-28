using System;
using System.ServiceModel;
using System.ServiceProcess;
using log4net;
using log4net.Config;

namespace CurrentAccount.WinService
{
    /// <summary>
    /// Сервис
    /// </summary>
    partial class ProcessDispatcher : ServiceBase
    {
        #region Logger

        private static readonly ILog log = LogManager.GetLogger(typeof(ProcessDispatcher));

        /// <summary>
        /// Объект - логер
        /// </summary>
        public static ILog Log { get { return log; } }

        /// <summary>
        /// инициализация логера
        /// </summary>
        public static void InitializeLogger() { XmlConfigurator.Configure(); }

        #endregion

        static ProcessDispatcher()
        {
            ProcessDispatcher.InitializeLogger();
        }

        private ProcessService service;
        private ServiceHost host;

        public ProcessDispatcher()
        {
            InitializeComponent();
        }
        
        /// <summary>
        /// Запуск сервиса
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            Log.Info("********************************************************************************************************************");
            Log.Info("Запуск сервиса");
            Log.Info("********************************************************************************************************************");

            try
            {
                
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
                service = new ProcessService();
                host = new ServiceHost(service);
                host.Open();

                Log.Info("Сервис запущен успешно");
            }
            catch (Exception exc)
            {
                Log.FatalFormat("Ошибка запуска сервиса: {0}. Trace: {1}", exc.Message, exc);
                Stop();
            }
        }

        /// <summary>
        /// Перехват необработанных исключений и запись в лог
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.FatalFormat("UnhandledException: ExceptionObject = {0}, IsTerminating = {1}", e.ExceptionObject, e.IsTerminating);
        }

        /// <summary>
        /// Остановка сервиса
        /// </summary>
        protected override void OnStop()
        {
            try
            {
                service.StopAllProcesses();
                service.SchedulerThread.Abort();
                host.Close();
                Log.Info("Сервис остановлен.");
            }
            catch (Exception exc)
            {
                Log.FatalFormat("Ошибка запуска сервиса: {0}. Trace: {1}", exc.Message, exc);
            }
        }
    }
}
