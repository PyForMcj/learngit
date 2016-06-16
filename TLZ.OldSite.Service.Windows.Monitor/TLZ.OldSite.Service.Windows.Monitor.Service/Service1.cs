using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using TLZ.Helper;
using TLZ.MongoDB;
using TLZ.OldSite.DB.MongoDB.Model;
using TLZ.OldSite.Service.Windows.Monitor.Host;

namespace TLZ.OldSite.Service.Windows.Monitor.Service
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
            threadService.MonitorPerformance();
        }
        protected override void OnStop()
        {
            threadService.Dispose();
        }
    }
}