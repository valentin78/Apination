using System.ServiceProcess;

namespace ApinationGateway
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new SheduleService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
