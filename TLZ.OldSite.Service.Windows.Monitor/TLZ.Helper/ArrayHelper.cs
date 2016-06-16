#region << 版 本 注 释 >>
/*
     * ========================================================================
     * Copyright Notice © 2010-2014 TideBuy.com All rights reserved .
     * ========================================================================
     * 机器名称：USER-429236GLDJ 
     * 文件名：  ArrayHelper 
     * 版本号：  V1.0.0.0 
     * 创建人：  wangyp 
     * 创建时间：2014/11/30 9:15:05 
     * 描述    :
     * =====================================================================
     * 修改时间：2014/11/30 9:15:05 
     * 修改人  ：  
     * 版本号  ： V1.0.0.0 
     * 描述    ：
*/
#endregion
using System.Collections.Generic;
using System.Linq;

namespace TLZ.Helper
{
    public class ArrayHelper
    {
        /// <summary>
        /// 获取两个数组或集合存在的交集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ones"></param>
        /// <param name="twos"></param>
        /// <returns>返回交集部分</returns>
        public static List<T> HasSameItem<T>(IEnumerable<T> ones, IEnumerable<T> twos)
        {
            return ones.Intersect<T>(twos).ToList<T>();
        }

        /// <summary>
        /// 获取两个数组或集合存在的差集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ones"></param>
        /// <param name="twos"></param>
        /// <returns>返回交集部分</returns>
        public static List<T> HasNoSameItem<T>(IEnumerable<T> ones, IEnumerable<T> twos)
        {
            return ones.Except<T>(twos).ToList<T>();
        }

        /// <summary>
        /// 获取两个数组或集合存在的并集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ones"></param>
        /// <param name="twos"></param>
        /// <returns>返回并集部分</returns>
        public static List<T> HasUnionItem<T>(IEnumerable<T> ones, IEnumerable<T> twos)
        {
            return ones.Union<T>(twos).ToList<T>();
        }
    }
}
