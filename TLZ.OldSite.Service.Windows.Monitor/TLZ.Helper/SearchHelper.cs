#region << 版 本 注 释 >>
/*
     * ========================================================================
     * Copyright Notice © 2010-2014 TideBuy.com All rights reserved .
     * ========================================================================
     * 机器名称：USER-429236GLDJ 
     * 文件名：  SearchHelper 
     * 版本号：  V1.0.0.0 
     * 创建人：  wangyunpeng 
     * 创建时间：2014/12/8 12:11:34 
     * 描述    : 获取关于PPC和搜索引擎的信息
     * =====================================================================
     * 修改时间：2014/12/8 12:11:34 
     * 修改人  ：  
     * 版本号  ： V1.0.0.0 
     * 描述    ：
*/
#endregion
using Microsoft.JScript;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace TLZ.Helper
{
    /// <summary>
    /// 获取关于PPC和搜索引擎的信息
    /// </summary>
    public class SearchHelper
    {
        #region 常量
        /// <summary>
        /// URL中表示SEM跟踪标识
        /// </summary>
        public const string PARAMETER_TRACK = "track";
        /// <summary>
        /// URL中表示客服ID的参数名称
        /// </summary>
        public const string PARAMETER_CSID = "csid";
        /// <summary>
        /// URL中表示币种的参数名称
        /// </summary>
        public const string PARAMETER_CURRENCY = "currency";
        public const string SEARCH_NAME = "enginee";
        public const string SEARCH_KEY = "keyword";
        public const string COOKIE_PREFIX = "url_param_";
        #endregion

        #region 变量
        /// <summary>
        /// 搜索引擎特征
        /// </summary>
        private readonly static string[][] _Enginers = new string[][] { 
            new string[]{"google","q"},
            new string[]{"bing","q"},
            new string[]{"m.baidu","wd|word|bs"},
            new string[]{"baidu","wd|word|bs"},
            new string[]{"yahoo","p"},
            new string[]{"live","q"}
        };
        /// <summary>
        /// 屏蔽的搜索引擎网址
        /// </summary>
        private readonly static string[] BlackEnginers = new string[] { 
            "cpro.baidu.com"
        };
        /// <summary>
        /// 搜索引擎的蜘蛛名称
        /// </summary>
        private readonly static string[] _Spiders = new string[]{
            "Googlebot"
            ,"Yahoo!"
            ,"Baiduspider"
            ,"Slurp"
            ,"Msnbot"
        };
        /// <summary>
        /// 搜索引擎的蜘蛛名称
        /// </summary>
        public static string[] Spiders
        {
            get { return SearchHelper._Spiders; }
        }
        private static readonly string DOMAIN_NAME = null;
        #endregion

        static SearchHelper()
        {
            DOMAIN_NAME = System.Web.Security.FormsAuthentication.CookieDomain;
        }

        #region 方法
        /// <summary>
        /// 获取Track信息
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="url">去掉track之后的url</param>
        /// <returns>是否包含track信息</returns>
        public static bool GetTrack(HttpRequest request, HttpResponse response, out string url)
        {
            bool isExist = false;
            url = null;
            string tracking = request.QueryString[PARAMETER_TRACK];
            string csid = request.QueryString[PARAMETER_CSID];
            string currency = request.QueryString[PARAMETER_CURRENCY];
            if (!string.IsNullOrWhiteSpace(tracking)
                || !string.IsNullOrWhiteSpace(csid)
                || !string.IsNullOrWhiteSpace(currency))
            {
                isExist = true;
                if (!string.IsNullOrWhiteSpace(tracking))
                {
                    HttpCookie cookieTrack = new HttpCookie(COOKIE_PREFIX + PARAMETER_TRACK);
                    cookieTrack.Value = Microsoft.Security.Application.Encoder.UrlEncode(tracking);
                    if (!string.IsNullOrEmpty(DOMAIN_NAME))
                    {
                        cookieTrack.Domain = DOMAIN_NAME;
                    }
                    response.Cookies.Add(cookieTrack);
                }
                if (!string.IsNullOrWhiteSpace(csid))
                {
                    HttpCookie cookieCsid = new HttpCookie(COOKIE_PREFIX + PARAMETER_CSID);
                    cookieCsid.Value = Microsoft.Security.Application.Encoder.UrlEncode(csid);
                    if (!string.IsNullOrEmpty(DOMAIN_NAME))
                    {
                        cookieCsid.Domain = DOMAIN_NAME;
                        cookieCsid.Expires = DateTime.Now.AddMonths(1);
                    }
                    response.Cookies.Add(cookieCsid);
                }
                if (!string.IsNullOrWhiteSpace(currency))
                {
                    HttpCookie cookieCurrency = new HttpCookie(COOKIE_PREFIX + PARAMETER_CURRENCY);
                    cookieCurrency.Value = Microsoft.Security.Application.Encoder.UrlEncode(currency);
                    if (!string.IsNullOrEmpty(DOMAIN_NAME))
                    {
                        cookieCurrency.Domain = DOMAIN_NAME;
                        cookieCurrency.Expires = DateTime.Now.AddMonths(1);
                    }
                    response.Cookies.Add(cookieCurrency);
                }
                NameValueCollection parameters = new NameValueCollection(request.QueryString.Count);
                foreach (string key in request.QueryString)
                {
                    if (!string.Equals(key, PARAMETER_TRACK, StringComparison.CurrentCultureIgnoreCase)
                        && !string.Equals(key, PARAMETER_CSID, StringComparison.CurrentCultureIgnoreCase)
                        && !string.Equals(key, PARAMETER_CURRENCY, StringComparison.CurrentCultureIgnoreCase))
                    {
                        parameters.Add(key, request.QueryString[key]);
                    }
                }
                url = SearchHelper.BuildUrl(request.Url.Host, request.Url.AbsolutePath, parameters);
                parameters.Clear();
                parameters = null;
            }
            else
            {
                url = request.Url.PathAndQuery.ToLower();
            }
            Uri uriReferrer = request.UrlReferrer;
            if (uriReferrer != null)
            {
                string engineeName = null, keyword = null;
                SearchHelper.GetSearch(out engineeName, out keyword, uriReferrer.ToString());
                if (!string.IsNullOrEmpty(engineeName) && !string.IsNullOrEmpty(keyword))
                {
                    HttpCookie cookieSearch = new HttpCookie(COOKIE_PREFIX + SEARCH_NAME);
                    cookieSearch.Value = Microsoft.Security.Application.Encoder.UrlEncode(engineeName);
                    response.Cookies.Add(cookieSearch);
                    HttpCookie cookieKeyWord = new HttpCookie(COOKIE_PREFIX + SEARCH_KEY);
                    cookieKeyWord.Value = Microsoft.Security.Application.Encoder.UrlEncode(keyword);
                    response.Cookies.Add(cookieKeyWord);
                }
            }
            return isExist;
        }
        /// <summary>
        /// 构建Url地址栏参数的方法(id=123&name=wyp)
        /// </summary>
        /// <param name="parameters">打开页面请求的参数</param>
        /// <returns>id=123&name=wyp</returns>
        private static string BuildQueryString(NameValueCollection parameters)
        {
            string queryString = null;
            List<string> pairs = new List<string>(parameters.Count);
            foreach (string key in parameters.Keys)
            {
                string value = parameters[key];
                if (!string.IsNullOrEmpty(value))
                {
                    pairs.Add(string.Format("{0}={1}", Uri.EscapeDataString(key.Trim().ToLower()), Microsoft.Security.Application.Encoder.UrlEncode(value.Trim().ToLower())));
                }
            }
            queryString = string.Join("&", pairs.ToArray());
            pairs.Clear();
            pairs = null;
            return queryString;
        }
        /// <summary>
        /// 构建Url完整的地址(传统方式的地址栏传参格式)
        /// </summary>
        /// <param name="webSiteUrl">网站路径（如：www.tbdress.com）</param>
        /// <param name="absoluteUrl">绝对路径（如：/meinv.aspx）</param>
        /// <param name="parameters">打开页面请求的参数</param>
        /// <returns>http://www.tbdress.com/meinv.aspx?id=123&name=wyp</returns>
        private static string BuildUrl(string webSiteUrl, string absoluteUrl, NameValueCollection parameters)
        {
            UriBuilder uri = new UriBuilder(webSiteUrl + absoluteUrl.Trim().ToLower());
            uri.Query = SearchHelper.BuildQueryString(parameters);
            return uri.Uri.ToString();
        }
        /// <summary>
        /// 获得搜索引擎信息
        /// </summary>
        /// <param name="engineeName"></param>
        /// <param name="Keyword"></param>
        /// <param name="url"></param>
        private static void GetSearch(out string engineeName, out string keyWord, string url)
        {
            engineeName = string.Empty;
            keyWord = string.Empty;
            url = url.ToLower();
            string[] blackEnginers = SearchHelper.BlackEnginers;
            for (int i = 0; i < blackEnginers.Length; i++)
            {
                string value = blackEnginers[i];
                if (url.Contains(value))
                {
                    return;
                }
            }
            string[] searchEnginers = null;
            string host = new Uri(url).Host;
            string[][] enginers = SearchHelper._Enginers;
            for (int i = 0; i < enginers.Length; i++)
            {
                if (host.Contains(enginers[i][0]))
                {
                    searchEnginers = enginers[i];
                    break;
                }
            }
            if (searchEnginers != null)
            {
                engineeName = searchEnginers[0];
                string query = searchEnginers[1];
                string[] querys;
                if (!query.Contains("|"))
                {
                    querys = new string[] { query };
                }
                else
                {
                    querys = query.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                }
                string arg = null;
                Regex regex = null;
                for (int i = 0; i < querys.Length; i++)
                {
                    try
                    {
                        arg = querys[i];
                    }
                    catch (IndexOutOfRangeException)
                    {
                        arg = null;
                    }
                    if (arg == null)
                        continue;
                    regex = new Regex(string.Format("({0}\\.+.*[?/&/#]{1}[=:])(?<key>[^&]*)", engineeName, arg), RegexOptions.IgnoreCase);
                    if (regex.IsMatch(url))
                    {
                        Match match = regex.Match(url);
                        string value = match.Groups["key"].Value;
                        try
                        {
                            keyWord = GlobalObject.decodeURIComponent(value);
                        }
                        catch (JScriptException)
                        {
                            keyWord = HttpUtility.UrlDecode(value, Encoding.GetEncoding("gb2312"));
                        }
                        if (!string.IsNullOrWhiteSpace(keyWord))
                        {
                            break;
                        }
                    }
                }
                regex = null;
                Array.Clear(querys, 0, querys.Length);
                querys = null;
            }
            keyWord = keyWord.Trim();
            if (string.IsNullOrEmpty(engineeName) && !string.IsNullOrEmpty(keyWord))
            {
                keyWord = string.Empty;
            }
            else if (!string.IsNullOrEmpty(engineeName) && string.IsNullOrEmpty(keyWord))
            {
                engineeName = string.Empty;
            }
        }
        #endregion
    }
}
