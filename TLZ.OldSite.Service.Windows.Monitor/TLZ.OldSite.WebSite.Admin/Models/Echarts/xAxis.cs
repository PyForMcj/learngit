using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TLZ.OldSite.WebSite.Admin.Models
{
    /// <summary>
    /// echarts中的xAxis
    /// </summary>
    public class xAxis
    {
        public string type { get; set; }

        public bool? boundaryGap { get; set; }

        public string[] data { get; set; }

        public xAxis() { }

        public xAxis(string Type, bool BoundaryGap, string[] Data)
        {
            this.type = Type;
            this.boundaryGap = BoundaryGap;
            this.data = Data;
        }
    }
}