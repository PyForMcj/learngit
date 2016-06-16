#region << 版 本 注 释 >>
/*
     * ========================================================================
     * Copyright Notice © 2010-2014 TideBuy.com All rights reserved .
     * ========================================================================
     * 机器名称：USER-429236GLDJ 
     * 文件名：  TranslatorHelper 
     * 版本号：  V1.0.0.0 
     * 创建人：  王云鹏 
     * 创建时间：2014/12/30 11:30:05 
     * 描述    : 翻译辅助类-google翻译，从老站拿过来改造一下。
     * =====================================================================
     * 修改时间：2014/12/30 11:30:05 
     * 修改人  ：  
     * 版本号  ： V1.0.0.0 
     * 描述    ：
*/
#endregion
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace TLZ.Helper
{
    /// <summary>
    /// 翻译辅助类
    /// </summary>
    public static class TranslatorHelper
    {
        private static readonly int TranslatorSleepTime = 0;
        public static string BingClientId = "wwwdresswecom";
        public static string BingClientSecret = "P3QaGA8A6vWjk6z5J9hDvF4M3K3pz5bCb1OPjcWqJJA=";
        /// <summary>
        /// 1 为Google 2 为bing
        /// </summary>
        private static readonly int TranslatorUserMethod = 0;
        static TranslatorHelper()
        {
            TranslatorSleepTime = Math.Max(TypeParseHelper.StrToInt32(ConfigHelper.GetAppSettingValue("TranslatorSleepTime")), 1);
            TranslatorUserMethod = Math.Max(TypeParseHelper.StrToInt32(ConfigHelper.GetAppSettingValue("TranslatorUserMethod")), 1);
            if (TranslatorSleepTime < 1)
                TranslatorSleepTime = 1;
            if (TranslatorUserMethod < 1)
                TranslatorUserMethod = 1;
        }
        /// <summary>
        /// 执行翻译,默认源语言是英语
        /// </summary>
        /// <param name="text"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static string Translator(string text, string to)
        {
            return Translator(text, "en", to);
        }
        public static string Translator(string text, string from, string to)
        {
            if (TranslatorUserMethod == 1)
            {
                text = Google.Translate(text, from, to);
            }
            else if (TranslatorUserMethod == 2)
            {
                text = BingTranslator.Translate(text, from, to.ToLower());
            }
            return text;
        }

        /// <summary>
        /// Google翻译
        /// </summary>
        public static class Google
        {
            /// <summary>
            /// 判断字符串中是否包含html标记
            /// </summary>
            private static readonly Regex _RegexHtmlTag = new Regex(@"<[^>]*>", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
            /// <summary>
            /// 默认使用的源语言
            /// </summary>
            private const string SOURCE_LANGUAGE = "auto";
            /// <summary>
            /// 翻译
            /// </summary>
            /// <param name="sourceText">源词</param>
            /// <param name="targetLanguageCode">要翻译的语言</param>
            /// <returns></returns>
            public static string Translate(string sourceText, string targetLanguageCode)
            {
                //google 翻译有一个很奇怪的bug，就是有些单词或者句子中包含大写字母时，有些语言会翻译不出来，顾全部强制用小写来翻译。
                return Translate(sourceText, SOURCE_LANGUAGE, targetLanguageCode);
            }
            /// <summary>
            /// 翻译
            /// </summary>
            /// <param name="sourceText">源词</param>
            /// <param name="sourceLanguageCode">源词的语言</param>
            /// <param name="targetLanguageCode">要翻译的语言</param>
            /// <returns></returns>
            public static string Translate(string sourceText, string sourceLanguageCode, string targetLanguageCode)
            {
                string targetText = string.Empty;
                if (string.IsNullOrEmpty(sourceText))
                    return targetText;
                sourceText = sourceText.ToLower().Replace("&nbsp;", " ");
                sourceLanguageCode = sourceLanguageCode.ToLower();
                targetLanguageCode = targetLanguageCode.ToLower();
                StringBuilder sb = new StringBuilder();
                MatchCollection matchCollection = _RegexHtmlTag.Matches(sourceText);
                int matchCollectionCount = matchCollection.Count;
                if (matchCollectionCount > 0)
                {
                    int offset = 0;
                    Match match = null;
                    for (int i = 0; i < matchCollectionCount; i++)
                    {
                        match = matchCollection[i];
                        if (offset != match.Index)
                        {
                            targetText = TranslateDo(sourceText.Substring(offset, match.Index - offset), sourceLanguageCode, targetLanguageCode);
                            sb.Append(targetText);
                        }
                        sb.Append(match.Value);
                        offset = match.Index + match.Length;
                    }
                    if (offset != sourceText.Length - 1)
                    {
                        targetText = TranslateDo(sourceText.Substring(offset), sourceLanguageCode, targetLanguageCode);
                        sb.Append(targetText);
                    }
                    match = null;
                }
                else
                {
                    targetText = TranslateDo(sourceText, sourceLanguageCode, targetLanguageCode);
                    sb.Append(targetText);
                }
                matchCollection = null;
                targetText = sb.ToString();
                sb.Clear();
                sb = null;
                return targetText;
            }

            /// <summary>
            /// 执行翻译请求
            /// </summary>
            /// <param name="sourceText"></param>
            /// <param name="sourceLanguageCode"></param>
            /// <param name="targetLanguageCode"></param>
            /// <returns></returns>
            private static string TranslateDo(string sourceText, string sourceLanguageCode, string targetLanguageCode)
            {
                string targetText = string.Empty;
                if (string.IsNullOrEmpty(sourceText))
                    return targetText;

                const string URL = "http://translate.google.cn/translate_a/single?client=t&sl={0}&tl={1}&hl=en&dt=bd&dt=ex&dt=ld&dt=md&dt=qca&dt=rw&dt=rm&dt=ss&dt=t&dt=at&ie=UTF-8&oe=UTF-8&otf=2&ssel=0&tsel=0&tk=519406|838853&q=";
                string url = string.Format(URL, sourceLanguageCode, targetLanguageCode);
                string json = null, parameter = null;
                JArray jArray = null;
                StringBuilder sb = new StringBuilder();
                List<string> list = sourceText.Length > 800 ? SplitStatement(sourceText) : new List<string>() { sourceText };
                foreach (string item in list)
                {
                    if (string.IsNullOrEmpty(item))
                        continue;
                    parameter = HttpUtilityHelper.UrlEncode(item);
                    try
                    {
                        json = WebAPIHelper.Get(string.Concat(url, parameter));
                        if (!string.IsNullOrEmpty(json))
                        {
                            jArray = JsonHelper.ConvertStrToJson<JArray>(json);
                            if (jArray != null && jArray[0] != null)
                            {
                                foreach (JArray strings in jArray[0])
                                {
                                    sb.Append(strings[0]);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    System.Threading.Thread.Sleep(TranslatorHelper.TranslatorSleepTime);
                }
                list.Clear();
                list = null;
                targetText = sb.ToString();
                sb.Clear();
                sb = null;
                return targetText;
            }

            /// <summary>
            /// 按标点符号分割返回字符串数组
            /// </summary>
            /// <param name="sourceText">要分割的句子</param>
            /// <returns>按标点符号分割返回字符串数组</returns>
            private static List<string> SplitStatement(string sourceText)
            {
                List<string> list = null;
                if (sourceText.Length < 3)
                {
                    list = new List<string>() { sourceText };
                }
                else
                {
                    list = new List<string>();
                    int offsetStart = 0, offsetEnd = 0;
                    while ((offsetStart = sourceText.IndexOfAny(new[] { '!', '.', '?', /*',', ';', ':'*/ }, offsetEnd)) > -1)
                    {
                        list.Add(string.Concat(sourceText.Substring(offsetEnd, offsetStart - offsetEnd), sourceText[offsetStart]));
                        offsetEnd = offsetStart + 1;
                    }
                    if (offsetEnd < sourceText.Length)
                    {
                        list.Add(sourceText.Substring(offsetEnd, sourceText.Length - offsetEnd));
                    }
                }
                return list;
            }
        }


        public class BingTranslator
        {
            public static string Translate(string text, string from, string to)
            {
                string uri = "http://api.microsofttranslator.com/v2/Http.svc/Translate?text=" + System.Web.HttpUtility.UrlEncode(text) + "&to=" + to;
                string translation = GetResponseByUri(uri);
                if (translation == text)
                {
                    //可能源语言不是英语的.检查一下源语言
                    string urlDetect = "http://api.microsofttranslator.com/V2/Http.svc/Detect?text=" + System.Web.HttpUtility.UrlEncode(text) + "";
                    string detect = GetResponseByUri(urlDetect);
                    uri = "http://api.microsofttranslator.com/v2/Http.svc/Translate?text=" + System.Web.HttpUtility.UrlEncode(text) + "&from=" + detect + "&to=" + to;
                    translation = GetResponseByUri(uri);
                }
                return translation;
            }

            public static string GetResponseByUri(string uri)
            {
                AdmAccessToken admToken;
                string headerValue = "";
                AdmAuthentication admAuth = new AdmAuthentication(BingClientId, BingClientSecret);
                try
                {
                    admToken = admAuth.GetAccessToken();
                    headerValue = "Bearer " + admToken.access_token;
                }
                catch (WebException)
                {
                }
                string response = "";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
                httpWebRequest.Headers.Add("Authorization", headerValue);
                WebResponse webResponse = null;
                try
                {
                    webResponse = httpWebRequest.GetResponse();
                    using (Stream stream = webResponse.GetResponseStream())
                    {
                        System.Runtime.Serialization.DataContractSerializer dcs = new System.Runtime.Serialization.DataContractSerializer(Type.GetType("System.String"));
                        response = (string)dcs.ReadObject(stream);
                    }
                }
                catch (Exception)
                {
                }
                finally
                {
                    if (webResponse != null)
                    {
                        webResponse.Close();
                        webResponse = null;
                    }
                }
                return response;
            }
        }
    }
    public class AdmAccessToken
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string expires_in { get; set; }
        public string scope { get; set; }
    }

    public class AdmAuthentication
    {
        public static readonly string DatamarketAccessUri = "https://datamarket.accesscontrol.windows.net/v2/OAuth2-13";
        private string clientId;
        private string cientSecret;
        private string request;

        public AdmAuthentication(string clientId, string clientSecret)
        {
            this.clientId = clientId;
            this.cientSecret = clientSecret;
            //If clientid or client secret has special characters, encode before sending request
            this.request = string.Format("grant_type=client_credentials&client_id={0}&client_secret={1}&scope=http://api.microsofttranslator.com", HttpUtility.UrlEncode(clientId), HttpUtility.UrlEncode(clientSecret));
        }

        public AdmAccessToken GetAccessToken()
        {
            return HttpPost(DatamarketAccessUri, this.request);
        }

        private AdmAccessToken HttpPost(string DatamarketAccessUri, string requestDetails)
        {
            //Prepare OAuth request 
            WebRequest webRequest = WebRequest.Create(DatamarketAccessUri);
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.Method = "POST";
            byte[] bytes = Encoding.ASCII.GetBytes(requestDetails);
            webRequest.ContentLength = bytes.Length;
            using (Stream outputStream = webRequest.GetRequestStream())
            {
                outputStream.Write(bytes, 0, bytes.Length);
            }
            using (WebResponse webResponse = webRequest.GetResponse())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(AdmAccessToken));
                //Get deserialized object from JSON stream
                AdmAccessToken token = (AdmAccessToken)serializer.ReadObject(webResponse.GetResponseStream());
                return token;
            }
        }
    }
}
