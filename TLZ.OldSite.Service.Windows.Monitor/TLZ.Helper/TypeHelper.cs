#region << 版 本 注 释 >>
/*
     * ========================================================================
     * Copyright Notice © 2010-2015 TideBuy.com All rights reserved .
     * ========================================================================
     * 机器名称：USER-429236GLDJ 
     * 文件名：  TypeHelper 
     * 版本号：  V1.0.0.0 
     * 创建人：  wangyunpeng 
     * 创建时间：2015/4/29 14:35:39 
     * 描述    :
     * =====================================================================
     * 修改时间：2015/4/29 14:35:39 
     * 修改人  ：Administrator
     * 版本号  ： V1.0.0.0 
     * 描述    ：
*/
#endregion
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace TLZ.Helper
{
    public class TypeHelper
    {
        /// <summary>
        /// 获得枚举描述
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDescription(MemberInfo value)
        {
            string result = null;
            DescriptionAttribute attribute = value.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault() as DescriptionAttribute;
            result = attribute != null ? attribute.Description : value.ToString();
            return result;
        }
    }
}
