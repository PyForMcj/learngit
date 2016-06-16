using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TLZ.OldSite.WebSite.Admin.Models
{
    /// <summary>
    /// echarts中的yAxis
    /// </summary>
    public class yAxis
    {
        public string type { get; set; }

        public yAxis() { }

        public yAxis(string Type)
        {
            this.type = Type;
        }
    }
}