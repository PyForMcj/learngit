using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TLZ.Logger
{
    public enum ELogLevel
    {
        /// <summary>
        /// 错误信息
        /// </summary>
        Error = 1,
        /// <summary>
        /// 跟踪信息
        /// </summary>
        Trace = 2,
        /// <summary>
        /// 调试信息
        /// </summary>
        Debug = 3,
        /// <summary>
        /// 跟踪信息
        /// </summary>
        Info = 4
    }
}
