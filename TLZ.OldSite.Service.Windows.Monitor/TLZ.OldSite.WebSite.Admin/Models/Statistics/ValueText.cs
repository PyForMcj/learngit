using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TLZ.OldSite.WebSite.Admin.Models
{
    public class ValueText
    {

        public string Text { get; set; }

        /// <summary>
        /// 主值
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// 副值
        /// </summary>
        public double ViceValue { get; set; }
    }
}