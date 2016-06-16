#region << 版 本 注 释 >>
/*
     * ========================================================================
     * Copyright Notice © 2010-2014 TideBuy.com All rights reserved .
     * ========================================================================
     * 机器名称：YANGHUANWEN 
     * 文件名：  ImageHelper 
     * 版本号：  V1.0.0.0 
     * 创建人：  Administrator 
     * 创建时间：2014/10/13 18:26:52 
     * 描述    :
     * =====================================================================
     * 修改时间：2014/10/13 18:26:52 
     * 修改人  ：  
     * 版本号  ： V1.0.0.0 
     * 描述    ：
*/
#endregion
using System;

namespace TLZ.Helper
{
    public class ImageHelper
    {

        /// <summary>
        /// 获取产品图片列表下标
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static int GetImageIndex(string image)
        {
            int idx = 1;
            if (string.IsNullOrEmpty(image))
                return idx;
            if (image.EndsWith(".jpg") == false)
                return idx;
            if (image.IndexOf("_", StringComparison.Ordinal) == -1)
                return idx;
            image = image.Replace(".jpg", "");
            image = image.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries)[1];
            idx = int.Parse(image);
            return idx;

        }
    }
}
