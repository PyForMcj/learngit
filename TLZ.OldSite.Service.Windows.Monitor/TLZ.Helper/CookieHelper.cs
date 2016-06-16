#region << 版 本 注 释 >>
/*
     * ========================================================================
     * Copyright Notice © 2010-2014 TideBuy.com All rights reserved .
     * ========================================================================
     * 机器名称：USER-429236GLDJ 
     * 文件名：  CookieHelper 
     * 版本号：  V1.0.0.0 
     * 创建人：  Administrator 
     * 创建时间：2015/1/8 9:32:20 
     * 描述    :
     * =====================================================================
     * 修改时间：2015/1/8 9:32:20 
     * 修改人  ：  
     * 版本号  ： V1.0.0.0 
     * 描述    ：
*/
#endregion
using Microsoft.Security.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TLZ.Helper;

namespace TLZ.Helper
{
    public class CookieHelper
    {
        public static void AddCookie(string cookieName, string value, HttpResponse response)
        {
            HttpCookie cookie = new HttpCookie(cookieName);
            cookie.Value = Encoder.UrlEncode(value);
            response.Cookies.Add(cookie);
        }

        public static void AddCookie(string cookieName, string value, DateTime expireDays, HttpResponse response)
        {
            HttpCookie cookie = new HttpCookie(cookieName, Encoder.UrlEncode(value));
            cookie.Expires = expireDays;
            response.Cookies.Add(cookie);
        }

        public static void AddCookie(string cookieName, string value, DateTime expireDays, HttpResponse response, string path)
        {
            HttpCookie cookie = new HttpCookie(cookieName, Encoder.UrlEncode(value));
            cookie.Expires = expireDays;
            cookie.Path = path;
            response.Cookies.Add(cookie);
        }

        public static void AddCookie(string cookieName, string value, DateTime expireDays, HttpResponse response, string path, string domain)
        {
            HttpCookie cookie = new HttpCookie(cookieName, Encoder.UrlEncode(value));
            cookie.Expires = expireDays;
            cookie.Path = path;
            cookie.Domain = domain;
            response.Cookies.Add(cookie);
        }

        /// <summary>
        /// 获取Cookie
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cookieName"></param>
        /// <returns></returns>
        public static string GetCookieValue(HttpRequest request, string cookieName)
        {
            string value = null;
            HttpCookie cookie = request.Cookies[cookieName];
            if (cookie != null)
            {
                value = cookie.Value;
                if (!string.IsNullOrEmpty(value))
                {
                    value = System.Web.HttpUtility.UrlDecode(value);
                }
            }
            return value;
        }

        public static void DeleteCookie(string cookieName, HttpResponse response)
        {
            HttpCookie cookie = new HttpCookie(cookieName);
            cookie.Expires = DateTime.UtcNow.AddYears(-1);
            response.Cookies.Add(cookie);
        }
    }
}
