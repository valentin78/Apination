using System.ServiceProcess;

namespace Sage50Connector
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application
        /// </summary>
        static void Main()
        {
            var servicesToRun = new ServiceBase[]
            {
                new ScheduleService()
            };
            ServiceBase.Run(servicesToRun);
        }
    }
}
