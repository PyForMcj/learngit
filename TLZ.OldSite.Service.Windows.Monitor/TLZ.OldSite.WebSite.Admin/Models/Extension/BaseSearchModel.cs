using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TLZ.OldSite.WebSite.Admin.Models
{
    /// <summary>
    /// 条件筛选
    /// </summary>
    public class BaseSearchModel
    {
        /// <summary>
        /// 关键字
        /// </summary>
        public string SearchKeyWord { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        public DateTime SearchDateTime { get; set; }

        /// <summary>
        /// 表筛选 针对ping表
        /// </summary>
        public List<string> SearchBiao { get; set; }

        /// <summary>
        /// 库筛选 针对各个服务器
        /// </summary>
        public List<string> SearchKu { get; set; }
    }
}