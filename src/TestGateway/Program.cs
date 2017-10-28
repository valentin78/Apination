using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGateway
{
    class Program
    {
        #region Logger

        private static readonly ILog log = LogManager.GetLogger(typeof(Program));
        public static ILog Log { get { return log; } }
        public static void InitializeLogger() { XmlConfigurator.Configure(); }

        #endregion

        static void Main(string[] args)
        {
            InitializeLogger();

            Log.Info("Test");

        }
    }
}
