using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TLZ.OldSite.WebSite.Admin.Models
{
    /// <summary>
    /// echarts中的tooltip
    /// </summary>
    public class tooltip
    {
        public string trigger { get; set; }
        public tooltip() { }
        public tooltip(string Trigger)
        {
            this.trigger = Trigger;
        }
    }
}