#region << 版 本 注 释 >>
/*
     * ========================================================================
     * Copyright Notice © 2010-2014 TideBuy.com All rights reserved .
     * ========================================================================
     * 机器名称：YANGHUANWEN 
     * 文件名：  IPHelper 
     * 版本号：  V1.0.0.0 
     * 创建人：  杨焕文 
     * 创建时间：2014/12/1 19:59:03 
     * 描述    :操作IP地址相关 
     * =====================================================================
     * 修改时间：2014/12/1 19:59:03 
     * 修改人  ：  
     * 版本号  ： V1.0.0.0 
     * 描述    ：
*/
#endregion
using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;

namespace TLZ.Helper
{
    public class IPHelper
    {
        private static bool IsIpAddress(string str1)
        {
            if (string.IsNullOrEmpty(str1) || str1.Length < 7 || str1.Length > 15) return false;
            const string regformat = @"^\d{1,3}[\.]\d{1,3}[\.]\d{1,3}[\.]\d{1,3}$";
            Regex regex = new Regex(regformat, RegexOptions.IgnoreCase);
            return regex.IsMatch(str1);
        }
        public static string Ip
        {
            get
            {
                string result = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (!string.IsNullOrEmpty(result))
                {
                    //可能有代理   
                    if (result.IndexOf(".", StringComparison.Ordinal) == -1)    //没有"."肯定是非IPv4格式   
                        result = null;
                    else
                    {
                        if (result.IndexOf(",", StringComparison.Ordinal) != -1)
                        {
                            //有","，估计多个代理。取第一个不是内网的IP。   
                            result = result.Replace(" ", "").Replace("\"", "");
                            string[] temparyip = result.Split(",;".ToCharArray());
                            foreach (string t in temparyip)
                            {
                                if (IsIpAddress(t) && t.Substring(0, 3) != "10." && t.Substring(0, 7) != "192.168" && t.Substring(0, 7) != "172.16.")
                                {
                                    return t;    //找到不是内网的地址   
                                }
                            }
                        }
                        else if (IsIpAddress(result)) //代理即是IP格式   
                            return result;
                        else
                            result = null;    //代理中的内容 非IP，取IP   
                    }
                }
                if (string.IsNullOrEmpty(result))
                    result = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                if (string.IsNullOrEmpty(result))
                    result = HttpContext.Current.Request.UserHostAddress;
                if (result == "::1")
                    result = "127.0.0.1";
                return result;
            }
        }


        /// <summary>
        /// IP 转换进制
        /// </summary>
        /// <param name="strIpAddress"></param>
        /// <returns></returns>
        public static uint IpToNumber(string strIpAddress)
        {
            try
            {
                if (strIpAddress.Count(m => m == '.') != 3)
                {
                    return 0;
                }
                string[] strIpArray = strIpAddress.Split('.');
                var iIp = new uint[strIpArray.Length];
                for (int i = 0; i < strIpArray.Length; ++i)
                {
                    iIp[i] = uint.Parse(strIpArray[i]);
                }
                uint dwIp = (iIp[0] << 24) | (iIp[1] << 16) | (iIp[2] << 8) | iIp[3];
                return dwIp;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static string GetUserIp
        {
            get
            {
                string ipList;
                try
                {
                    ipList = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                    if (string.IsNullOrEmpty(ipList)) ipList = HttpContext.Current.Request.UserHostAddress;
                }
                catch
                {
                    ipList = HttpContext.Current.Request.UserHostAddress;
                }
                if (ipList != null)
                {
                    string[] ipArray = ipList.Split(',');
                    foreach (string ipAddress in ipArray)
                    {
                        if (IsPublicIp(ipAddress.Trim()))
                            return ipAddress;
                    }
                }
                return HttpContext.Current.Request.UserHostAddress;
            }
        }


        /// <summary>
        /// 将int型表示的IP还原成正常IPv4格式。
        /// </summary>
        /// <param name="intIpAddress">uint型表示的IP</param>
        /// <returns></returns>
        public static string NumberToIp(uint intIpAddress)
        {
            byte[] bs = BitConverter.GetBytes(intIpAddress);
            return string.Format("{0}.{1}.{2}.{3}", bs[3], bs[2], bs[1], bs[0]);
        }



        private static bool IsPublicIp(string ipAddress)
        {
            bool isPublic = true;

            try
            {
                string[] ips = ipAddress.Split('.');
                if (ips.Length < 4)
                {
                    return false;
                }
                int w = 0;
                int x = 0;
                int y = 0;
                int z = 0;
                int.TryParse(ips[0], out w);
                int.TryParse(ips[1], out x);
                int.TryParse(ips[2], out y);
                int.TryParse(ips[3], out z);

                if (w == 127 && x == 0 && y == 0 && z == 1) // 127.0.0.1
                {
                    isPublic = false;
                }
                else if (w == 10) // 10.0.0.0 - 10.255.255.255
                {
                    isPublic = false;
                }
                else if (w == 172 && (x >= 16 || x <= 31)) // 172.16.0.0 - 172.31.255.255
                {
                    isPublic = false;
                }
                else if (w == 192 && x == 168) // 192.168.0.0 - 192.168.255.255
                {
                    isPublic = false;
                }
            }
            catch
            {
                isPublic = false;
            }
            return isPublic;
        }
    }
}
