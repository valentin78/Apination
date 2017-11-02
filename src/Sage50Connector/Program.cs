﻿using System.ServiceProcess;

namespace Sage50Connector
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
                new ScheduleService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
