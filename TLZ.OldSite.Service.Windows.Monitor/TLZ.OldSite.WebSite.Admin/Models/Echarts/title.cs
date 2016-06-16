using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TLZ.OldSite.WebSite.Admin.Models
{
    /// <summary>
    /// echarts中的title
    /// </summary>
    public class title
    {
        /// <summary>
        /// text
        /// </summary>
        public string text { get; set; }

        public string x { get; set; }

        public string y { get; set; }

        public string textAlign { get; set; }

        public title() { }
        public title(string Text,string X,string Y, string TextAlign)
        {
            this.text = Text;
            this.x = X;
            this.y = Y;
            this.textAlign = TextAlign;
        }
    }
}