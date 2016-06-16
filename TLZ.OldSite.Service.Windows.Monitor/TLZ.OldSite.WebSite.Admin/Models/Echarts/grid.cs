using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TLZ.OldSite.WebSite.Admin.Models
{
    /// <summary>
    /// echarts中的grid
    /// </summary>
    public class grid
    {
        public string left { get; set; }

        public string right { get; set; }

        public string bottom { get; set; }

        public string top { get; set; }

        public bool containLabel { get; set; }

        public grid() { }

        public grid(string Left, string Right, string Bottom, string Top, bool ContainLabel)
        {
            this.left = Left;
            this.right = Right;
            this.bottom = Bottom;
            this.top = Top;
            this.containLabel = ContainLabel;
        }
    }
}