using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using TLZ.Helper;
using TLZ.MongoDB;
using TLZ.OldSite.DB.MongoDB.Model;
using System.Threading;
using TLZ.OldSite.Service.Windows.Monitor.Host;

namespace TLZ.OldSite.Service.Windows.Monitor.SyetemPingService
{
    public partial class Service1 : ServiceBase
    {
        ThreadMonitorService threadService;
        public Service1()
        {
            InitializeComponent();
            threadService = new ThreadMonitorService();
        }

        protected override void OnStart(string[] args)
        {
            threadService.MonitorPing();
        }

        protected override void OnStop()
        {
            threadService.Dispose();
        }
    }
}
