using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TLZ.OldSite.WebSite.Admin.Models
{
    /// <summary>
    /// echarts中的toolbox
    /// </summary>
    public class toolbox
    {
        public feature feature { get; set; }

        public toolbox() { }

        public toolbox(feature Feature)
        {
            this.feature = Feature;
        }
    }
}