using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TLZ.OldSite.WebSite.Admin.Models
{
    /// <summary>
    /// echarts中的series
    /// </summary>
    public class series
    {
        public string name { get; set; }

        public string type { get; set; }

        public string stack { get; set; }

        public double[] data { get; set; }

        public series() { }

        public series(string Name, string Type, string Stack, double[] Data)
        {
            this.name = Name;
            this.type = Type;
            this.stack = Stack;
            this.data = Data;
        }
    }
}