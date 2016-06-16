using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TLZ.OldSite.WebSite.Admin.Models
{
    /// <summary>
    /// echarts中的feature
    /// </summary>
    public class feature
    {
        public saveAsImage saveAsImage { get; set; }

        public feature() { }

        public feature(saveAsImage SaveAsImage)
        {
            this.saveAsImage = SaveAsImage;
        }
    }
}