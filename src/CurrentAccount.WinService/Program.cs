using System.ServiceProcess;

namespace CurrentAccount.WinService
{
    /// <summary>
    /// Тип реализующий логику приложения
    /// </summary>
    static class Program
    {
        /// <summary>
        /// Точка входа в приложение
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
			{ 
				new ProcessDispatcher() 
			};
            ServiceBase.Run(ServicesToRun);
        }
    }
}