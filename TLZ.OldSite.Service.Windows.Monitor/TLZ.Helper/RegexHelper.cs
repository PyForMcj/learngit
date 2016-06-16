#region << 版 本 注 释 >>
/*
     * ========================================================================
     * Copyright Notice © 2010-2014 TideBuy.com All rights reserved .
     * ========================================================================
     * 机器名称：USER-429236GLDJ 
     * 文件名：  RegexHelper 
     * 版本号：  V1.0.0.0 
     * 创建人：  wangyp 
     * 创建时间：2014/12/12 13:15:05 
     * 描述    :
     * =====================================================================
     * 修改时间：2014/12/12 13:15:05 
     * 修改人  ：  
     * 版本号  ： V1.0.0.0 
     * 描述    ：
*/
#endregion
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace TLZ.Helper
{
    /// <summary>
    /// 正则表达式辅助类
    /// </summary>
    public static class RegexHelper
    {
        private const RegexOptions OPTIONS = RegexOptions.IgnoreCase | RegexOptions.Compiled;

        public const string HTMLCOLOR = @"^#?([a-f]|[A-F]|[0-9]){3}(([a-f]|[A-F]|[0-9]){3})?$";
        private static readonly Regex _regexHtmlColor = new Regex(HTMLCOLOR, OPTIONS);

        public const string NUMBER = @"^[0-9]+$";
        private static readonly Regex _regexNumber = new Regex(NUMBER, OPTIONS);

        public const string ENGLISH = @"^[A-Za-z0-9]+$";
        private static readonly Regex _regexEnglish = new Regex(ENGLISH, OPTIONS);

        public const string EMAIL = @"^[\w-]+(\.[\w-]+)*@[\w-]+(\.[\w-]+)+$";
        private static readonly Regex _regexEmail = new Regex(EMAIL, OPTIONS);

        public const string IP = @"^\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}$";
        private static readonly Regex _regexIp = new Regex(IP, OPTIONS);

        public const string URL = @"^(http|https|ftp|rtsp|mms):(\/\/|\\\\)[A-Za-z0-9%\-_@]+\.[A-Za-z0-9%\-_@]+[A-Za-z0-9\.\/=\?%\-&_~`@:\+!;]*$";
        private static readonly Regex _regexUrl = new Regex(URL, OPTIONS);

        public const string CHINESE = @"^[\u4e00-\u9fa5]{2,}$";
        private static readonly Regex _regexChinese = new Regex(CHINESE, OPTIONS);

        private static readonly Regex _regexMobile = new Regex("AppleWebKit", OPTIONS);

        public const string PARAMETER_KEY_VALUE = "([^=&]+)=([^&]*)";
        private static readonly Regex _RegexParameterKeyValue = new Regex(PARAMETER_KEY_VALUE, OPTIONS);


        /// <summary>
        /// 判断是否网页颜色 如:#ffffff 不包含透明度
        /// </summary>
        /// <param name="strColor"></param>
        /// <returns></returns>
        public static bool IsHtmlColor(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return false;
            return _regexHtmlColor.IsMatch(str);
        }
        /// <summary>
        /// 判断是否为数组
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNumber(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return false;
            return _regexNumber.IsMatch(str);
        }
        /// <summary>
        /// 判断是否为电子邮件
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsEmail(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return false;
            return _regexEmail.IsMatch(str);
        }
        /// <summary>
        /// 判断是否为IP地址
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsIP(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return false;
            return _regexIp.IsMatch(str);
        }
        /// <summary>
        /// 判断是否为URL
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsUrl(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return false;
            return _regexUrl.IsMatch(str);
        }
        /// <summary>
        /// 判断是否为中文
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsChinese(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return false;
            return _regexChinese.IsMatch(str);
        }
        /// <summary>
        /// 判断是否为英文字母
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsEnglish(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return false;
            return _regexEnglish.IsMatch(str);
        }
        /// <summary>
        /// 判断是否为移动端上网的
        /// </summary>
        /// <param name="userAgent"></param>
        /// <returns>True表示移动端，False表示PC</returns>
        public static bool IsMobile(string userAgent)
        {
            if (string.IsNullOrWhiteSpace(userAgent))
                return false;
            return _regexMobile.IsMatch(userAgent);
        }
        /// <summary>
        /// 获取23=716&amp;24=750里面的所有键和值的集合
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static NameValueCollection GetParameterKeyValueCollection(string parameter)
        {
            NameValueCollection nameValueCollection = null;
            if (string.IsNullOrWhiteSpace(parameter))
            {
                return nameValueCollection;
            }
            MatchCollection mc = _RegexParameterKeyValue.Matches(parameter);
            if (mc.Count > 0)
            {
                nameValueCollection = new NameValueCollection(mc.Count);
                foreach (Match m in mc)
                {
                    for (int i = 0; i < m.Groups.Count; )
                    {
                        if (i % 3 == 0)
                        {
                            i++;
                            continue;
                        }
                        string key = m.Groups[i++].Value;
                        string value = m.Groups[i++].Value;
                        nameValueCollection.Add(key, value);
                    }
                }
            }
            return nameValueCollection;
        }

        // <summary>
        /// 移除Html标记
        /// </summary>
        /// <param name="content">含有html标签的网页内容</param>
        /// <returns>没有html标签的网页内容</returns>
        public static string RemoveHtml(string html)
        {
            return System.Text.RegularExpressions.Regex.Replace(html, "<[^>]*>", "").Replace("&nbsp;", " ");
        }

    }
}
