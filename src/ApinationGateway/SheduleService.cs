using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ApinationGateway
{
    public partial class SheduleService : ServiceBase
    {
        public SheduleService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Program.Log.Debug("OnStart");
        }

        protected override void OnStop()
        {
            Program.Log.Debug("OnStop");
        }
    }
}
