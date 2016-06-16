using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TLZ.OldSite.WebSite.Admin.Models
{
    /// <summary>
    /// echarts中的legend
    /// </summary>
    public class legend
    {
        public string[] data { get; set; }

        public legend() { }

        public legend(string[] Data)
        {
            this.data = Data;
        }
    }
}