#region << 版 本 注 释 >>
/*
     * ========================================================================
     * Copyright Notice © 2010-2014 TideBuy.com All rights reserved .
     * ========================================================================
     * 机器名称：USER-429236GLDJ 
     * 文件名：  WebAPIHelper 
     * 版本号：  V1.0.0.0 
     * 创建人：  wangyunpeng 
     * 创建时间：2014/11/30 10:26:15 
     * 描述    :
     * =====================================================================
     * 修改时间：2014/11/30 10:26:15 
     * 修改人  ：  
     * 版本号  ： V1.0.0.0 
     * 描述    ：
*/
#endregion
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;

namespace TLZ.Helper
{
    /// <summary>
    /// WebAPI封装
    /// winform:http://www.it165.net/pro/html/201306/6052.html
    /// asp.net mvc:http://www.cnblogs.com/parry/archive/2012/09/27/ASPNET_MVC_Web_API.html
    /// asp.net webform:http://www.asp.net/web-api/overview/getting-started-with-aspnet-web-api/using-web-api-with-aspnet-web-forms
    /// </summary>
    public class WebAPIHelper
    {
        /// <summary>
        /// http://www.it165.net/pro/html/201306/6052.html
        /// GET获取HTTP信息
        /// </summary>
        /// <param name="url"></param>
        /// <param name="isException">是否返回错误信息</param>
        /// <returns></returns>
        public static string Get(string url, bool isException = false)
        {
            string responseText = null;
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    responseText = client.GetStringAsync(url).Result;
                }
                catch (Exception ex)
                {
                    if (isException)
                    {
                        responseText = ex.ToString();
                    }
                    else
                    {
                        responseText = null;
                    }
                }
            }
            return responseText;
        }
        public static string GetGZip(string url, bool isException = false)
        {
            string responseText = null;
            using (HttpClientHandler handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip })
            {
                //handler.UseDefaultCredentials = true;
                //handler.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
                //handler.UseCookies = true;
                //handler.AllowAutoRedirect = true;

                //handler.CookieContainer.Add(new Cookie());
                using (HttpClient client = new HttpClient(handler))
                {
                    try
                    {
                        responseText = client.GetStringAsync(url).Result;
                    }
                    catch (Exception ex)
                    {
                        if (isException)
                        {
                            responseText = ex.ToString();
                        }
                        else
                        {
                            responseText = null;
                        }
                    }
                }
            }
            return responseText;
        }
        /// <summary>
        /// http://www.haogongju.net/art/1642652
        /// POST方式发送信息
        /// </summary>
        /// <param name="url"></param>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static string Post(string url, Dictionary<string, string> dict, bool isException = false)
        {
            string responseText = null;
            using (HttpClient client = new HttpClient())
            {
                using (FormUrlEncodedContent content = new FormUrlEncodedContent(dict))
                {
                    using (HttpResponseMessage responseMessage = client.PostAsync(url, content).Result)
                    {
                        responseMessage.EnsureSuccessStatusCode();
                        try
                        {
                            responseText = responseMessage.Content.ReadAsStringAsync().Result;
                        }
                        catch (Exception ex)
                        {
                            if (isException)
                            {
                                responseText = ex.ToString();
                            }
                            else
                            {
                                responseText = null;
                            }
                        }
                    }
                }
            }
            return responseText;
        }
        public static string PostGZip(string url, Dictionary<string, string> dict, bool isException = false)
        {
            string responseText = null;
            using (HttpClientHandler handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip })
            {
                using (HttpClient client = new HttpClient(handler))
                {
                    using (FormUrlEncodedContent content = new FormUrlEncodedContent(dict))
                    {
                        using (HttpResponseMessage responseMessage = client.PostAsync(url, content).Result)
                        {
                            responseMessage.EnsureSuccessStatusCode();
                            try
                            {
                                responseText = responseMessage.Content.ReadAsStringAsync().Result;
                            }
                            catch (Exception ex)
                            {
                                if (isException)
                                {
                                    responseText = ex.ToString();
                                }
                                else
                                {
                                    responseText = null;
                                }
                            }
                        }
                    }
                }
            }
            return responseText;
        }
    }
}
