using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TLZ.OldSite.WebSite.Admin.Models
{
    public class echartsOption
    {
        public title title { get; set; }

        public tooltip tooltip { get; set; }

        public legend legend { get; set; }

        public grid grid { get; set; }

        public toolbox toolbox { get; set; }

        public xAxis xAxis { get; set; }

        public yAxis yAxis { get; set; }

        public series[] series { get; set; }

        public echartsOption() { }

        public echartsOption(title Title, tooltip Tooltip, legend Legend, grid Grid, toolbox Toolbox, xAxis XAxis, yAxis YAxis, series[] Series)
        {
            this.title = Title;
            this.tooltip = Tooltip;
            this.legend = Legend;
            this.grid = Grid;
            this.toolbox = Toolbox;
            this.xAxis = XAxis;
            this.yAxis = YAxis;
            this.series = Series;
        }
    }
}