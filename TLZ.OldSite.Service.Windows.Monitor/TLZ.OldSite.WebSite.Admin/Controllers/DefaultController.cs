using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Newtonsoft.Json;
using TLZ.MongoDB;
using TLZ.OldSite.DB.MongoDB.Model;
using TLZ.OldSite.WebSite.Admin.Models;
using TLZ.OldSite.WebSite.Admin.Models.PublicAction;

namespace TLZ.OldSite.WebSite.Admin.Controllers
{
    [UserValidate]
    public class DefaultController : Controller
    {

        readonly string MongoDBUserName = ConfigurationManager.AppSettings["MongoDBUserName"].ToString();
        readonly string MongoDBUserPassWord = ConfigurationManager.AppSettings["MongoDBUserPassWord"].ToString();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DefaultHeader()
        {
            ViewBag.currentUserName = CurrentUserInfo.GetPersonName();
            return PartialView();
        }                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                   

        public ActionResult DefaultLeftMenu()
        {
            return PartialView();
        }

        public ActionResult DefaultIndex()
        {
            //用于筛选路径
            var searchAddress = new Dictionary<string, string>();
            searchAddress.Add("Controller", RouteData.Values["controller"].ToString());
            searchAddress.Add("Action", RouteData.Values["action"].ToString() + "zTreeCharts");
            ViewData["searchAddress"] = searchAddress;

            //获取zTree所需要数据
            var sortBy = SortBy<DataBaseServerConfigures>.Descending(item => item.DateTimeNow);
            var DataBaseServerConfiguresList = MongoDBHelper.Select<DataBaseServerConfigures>(null, sortBy, MongoConnType.Center);
            var zTreeSource = DataBaseServerConfiguresList.Select(item => new { name = item.DataBaseName });
            ViewData["zTreeSource"] = JsonConvert.SerializeObject(zTreeSource, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            return PartialView();
        }
        public ActionResult DefaultIndexzTreeCharts()
        {
            ViewData["dataBaseName"] = Request["DataBaseName"] ?? "";
            return PartialView();
        }

        #region 活动配置
        /// <summary>
        /// 活动配置
        /// </summary>
        /// <param name="searchkey">关键字搜索</param>
        /// <param name="index">当前页数</param>
        /// <returns></returns>
        public ActionResult DefaultActivityConfigures(string searchkey, string index)
        {
            #region 筛选条件
            var baseSearch = new BaseSearchModel();
            ViewData["baseSearchModel"] = baseSearch;
            #endregion

            //mongodb筛选条件
            List<IMongoQuery> queryList = new List<IMongoQuery>();
            IMongoQuery query = null;

            if (string.IsNullOrWhiteSpace(index))
            {
                index = "1";
            }
            if (!string.IsNullOrWhiteSpace(searchkey))
            {
                queryList.Add(Query<ActivityConfigures>.EQ(item => item.ActivityName, searchkey));
            }
            if (queryList.Count > 0)
            {
                query = Query.And(queryList.ToArray());
            }

            //mongodb排序
            var sortBy = SortBy<ActivityConfigures>.Descending(item => item.DateTimeNow);

            //mongodb获取全部记录数
            long recordCount;
            MongoDBHelper.Select<ActivityConfigures>(out recordCount, MongoConnType.Center);

            BasePageModel page = new BasePageModel() { SearchKeyWord = searchkey, ActionName = RouteData.Values["action"].ToString() + "Page", CurrentIndex = Int32.Parse(index), TotalCount = recordCount };
            var ActivityConfiguresList = MongoDBHelper.Select<ActivityConfigures>(page.PageSize, page.CurrentIndex, query, sortBy, MongoConnType.Center);
            ViewData["pagemodel"] = page;

            //用于筛选路径
            var searchAddress = new Dictionary<string, string>();
            searchAddress.Add("Controller", RouteData.Values["controller"].ToString());
            searchAddress.Add("Action", RouteData.Values["action"].ToString() + "Page");
            ViewData["searchAddress"] = searchAddress;
            return PartialView(ActivityConfiguresList);
        }
        public ActionResult DefaultActivityConfiguresPage(string searchkey, string index)
        {
            #region 筛选
            string searchKeyWord = Request["SearchKeyWord"] ?? "";
            string searchDateTimeStart = Request["SearchDateTimeStart"] ?? "";
            string searchDateTimeEnd = Request["SearchDateTimeEnd"] ?? "";
            #endregion

            //mongodb筛选条件
            List<IMongoQuery> queryList = new List<IMongoQuery>();
            IMongoQuery query = null;

            if (string.IsNullOrWhiteSpace(index))
            {
                index = "1";
            }
            if (!string.IsNullOrWhiteSpace(searchKeyWord))
            {
                queryList.Add(Query<ActivityConfigures>.Matches(item => item.ActivityName, searchKeyWord));
            }
            if (!string.IsNullOrWhiteSpace(searchDateTimeStart))
            {
                queryList.Add(Query<ActivityConfigures>.GTE(item => item.DateTimeNow, Convert.ToDateTime(searchDateTimeStart)));
            }
            if (!string.IsNullOrWhiteSpace(searchDateTimeEnd))
            {
                queryList.Add(Query<ActivityConfigures>.LTE(item => item.DateTimeNow, Convert.ToDateTime(searchDateTimeStart)));
            }
            if (queryList.Count > 0)
            {
                query = Query.And(queryList.ToArray());
            }

            //mongodb排序
            var sortBy = SortBy<ActivityConfigures>.Descending(item => item.DateTimeNow);

            //mongodb获取全部记录数
            long recordCount;
            MongoDBHelper.Select<ActivityConfigures>(query, out recordCount, MongoConnType.Center);

            BasePageModel page = new BasePageModel() { SearchKeyWord = searchkey, ActionName = RouteData.Values["action"].ToString(), CurrentIndex = Int32.Parse(index), TotalCount = recordCount };
            var ActivityConfiguresList = MongoDBHelper.Select<ActivityConfigures>(page.PageSize, page.CurrentIndex, query, sortBy, MongoConnType.Center);
            ViewData["pagemodel"] = page;
            return PartialView(ActivityConfiguresList);
        }


        #endregion

        #region 服务器配置
        /// <summary>
        /// 数据库和服务器配置
        /// </summary>
        /// <param name="searchkey">关键字搜索</param>
        /// <param name="index">当前页数</param>
        /// <returns></returns>
        public ActionResult DefaultDataBaseServerConfigures(string searchkey, string index)
        {
            #region 筛选条件
            var baseSearch = new BaseSearchModel();
            ViewData["baseSearchModel"] = baseSearch;
            #endregion

            //mongodb筛选条件
            List<IMongoQuery> queryList = new List<IMongoQuery>();
            IMongoQuery query = null;

            if (string.IsNullOrWhiteSpace(index))
            {
                index = "1";
            }
            if (!string.IsNullOrWhiteSpace(searchkey))
            {
                queryList.Add(Query<DataBaseServerConfigures>.EQ(item => item.DataBaseName, searchkey));
            }
            if (queryList.Count > 0)
            {
                query = Query.And(queryList.ToArray());
            }

            //mongodb排序
            var sortBy = SortBy<DataBaseServerConfigures>.Descending(item => item.DateTimeNow);

            //mongodb获取全部记录数
            long recordCount;
            MongoDBHelper.Select<DataBaseServerConfigures>(out recordCount, MongoConnType.Center);

            BasePageModel page = new BasePageModel() { SearchKeyWord = searchkey, ActionName = RouteData.Values["action"].ToString() + "Page", CurrentIndex = Int32.Parse(index), TotalCount = recordCount };
            var DataBaseServerConfiguresList = MongoDBHelper.Select<DataBaseServerConfigures>(page.PageSize, page.CurrentIndex, query, sortBy, MongoConnType.Center);
            ViewData["pagemodel"] = page;

            //用于筛选路径
            var searchAddress = new Dictionary<string, string>();
            searchAddress.Add("Controller", RouteData.Values["controller"].ToString());
            searchAddress.Add("Action", RouteData.Values["action"].ToString() + "Page");
            ViewData["searchAddress"] = searchAddress;
            return PartialView(DataBaseServerConfiguresList);
        }
        public ActionResult DefaultDataBaseServerConfiguresPage(string searchkey, string index)
        {
            #region 筛选
            string searchKeyWord = Request["SearchKeyWord"] ?? "";
            string searchDateTimeStart = Request["SearchDateTimeStart"] ?? "";
            string searchDateTimeEnd = Request["SearchDateTimeEnd"] ?? "";
            #endregion

            //mongodb筛选条件
            List<IMongoQuery> queryList = new List<IMongoQuery>();
            IMongoQuery query = null;

            if (string.IsNullOrWhiteSpace(index))
            {
                index = "1";
            }
            if (!string.IsNullOrWhiteSpace(searchKeyWord))
            {
                queryList.Add(Query<DataBaseServerConfigures>.Matches(item => item.DataBaseName, searchKeyWord));
            }
            if (!string.IsNullOrWhiteSpace(searchDateTimeStart))
            {
                queryList.Add(Query<DataBaseServerConfigures>.GTE(item => item.DateTimeNow, Convert.ToDateTime(searchDateTimeStart)));
            }
            if (!string.IsNullOrWhiteSpace(searchDateTimeEnd))
            {
                queryList.Add(Query<DataBaseServerConfigures>.LTE(item => item.DateTimeNow, Convert.ToDateTime(searchDateTimeStart)));
            }
            if (queryList.Count > 0)
            {
                query = Query.And(queryList.ToArray());
            }

            //mongodb排序
            var sortBy = SortBy<DataBaseServerConfigures>.Descending(item => item.DateTimeNow);

            //mongodb获取全部记录数
            long recordCount;
            MongoDBHelper.Select<DataBaseServerConfigures>(query, out recordCount, MongoConnType.Center);

            BasePageModel page = new BasePageModel() { SearchKeyWord = searchkey, ActionName = RouteData.Values["action"].ToString(), CurrentIndex = Int32.Parse(index), TotalCount = recordCount };
            var DataBaseServerConfiguresList = MongoDBHelper.Select<DataBaseServerConfigures>(page.PageSize, page.CurrentIndex, query, sortBy, MongoConnType.Center);
            ViewData["pagemodel"] = page;
            return PartialView(DataBaseServerConfiguresList);
        }

        #endregion

        #region 服务器ping

        /// <summary>
        /// 服务器ping配置
        /// </summary>
        /// <param name="searchkey"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public ActionResult DefaultSystenPing(string searchkey, string index)
        {
            //1.获取所有中心库所有表
            var baseSearch = new BaseSearchModel();
            var baseSearchContext = new List<string>();
            baseSearch.SearchBiao = MongoDBHelper.AllCollection(MongoConnType.Center);
            foreach (var item in baseSearch.SearchBiao)
            {
                if (item.StartsWith("SystemPing"))
                {
                    baseSearchContext.Add(item.Replace("SystemPing", ""));
                }
            }
            baseSearch.SearchBiao = baseSearchContext;
            ViewData["baseSearchModel"] = baseSearch;

            //此为多表数据，初始化无法确定加载哪一个，故此传NULL
            List<SystemPing> systemPingList = null;

            //用于筛选路径
            var searchAddress = new Dictionary<string, string>();
            searchAddress.Add("Controller", RouteData.Values["controller"].ToString());
            searchAddress.Add("Action", RouteData.Values["action"].ToString() + "Page");
            ViewData["searchAddress"] = searchAddress;
            return PartialView(systemPingList);
        }
        public ActionResult DefaultSystenPingPage(string searchkey, string index)
        {
            #region 筛选
            string searchKeyWord = Request["SearchKeyWord"] ?? "";
            string searchDateTimeStart = Request["SearchDateTimeStart"] ?? "";
            string searchDateTimeEnd = Request["SearchDateTimeEnd"] ?? "";
            string SearchBiao = Request["SearchBiao"] ?? "";
            #endregion

            //mongodb筛选条件
            List<IMongoQuery> queryList = new List<IMongoQuery>();
            IMongoQuery query = null;

            if (string.IsNullOrWhiteSpace(index))
            {
                index = "1";
            }
            if (!string.IsNullOrWhiteSpace(searchKeyWord))
            {
                queryList.Add(Query<SystemPing>.Matches(item => item.PingIP, searchKeyWord));
            }
            if (!string.IsNullOrWhiteSpace(searchDateTimeStart))
            {
                queryList.Add(Query<SystemPing>.GTE(item => item.DateTimeNow, Convert.ToDateTime(searchDateTimeStart)));
            }
            if (!string.IsNullOrWhiteSpace(searchDateTimeEnd))
            {
                queryList.Add(Query<SystemPing>.LTE(item => item.DateTimeNow, Convert.ToDateTime(searchDateTimeStart)));
            }
            if (queryList.Count > 0)
            {
                query = Query.And(queryList.ToArray());
            }

            //mongodb排序
            var sortBy = SortBy<SystemPing>.Descending(item => item.DateTimeNow);

            //mongodb获取全部记录数
            long recordCount;
            if (!MongoDBHelper.Exist("SystemPing" + SearchBiao, MongoConnType.Center))
            {
                List<SystemPing> systemList = null;
                return PartialView(systemList);
            }
            MongoDBHelper.Select<SystemPing>("SystemPing" + SearchBiao, query, out recordCount, MongoConnType.Center);

            BasePageModel page = new BasePageModel() { SearchKeyWord = searchkey, ActionName = RouteData.Values["action"].ToString(), CurrentIndex = Int32.Parse(index), TotalCount = recordCount };
            var systemPingList = MongoDBHelper.Select<SystemPing>("SystemPing" + SearchBiao, page.PageSize, page.CurrentIndex, query, sortBy, MongoConnType.Center);
            ViewData["pagemodel"] = page;
            return PartialView(systemPingList);
        }

        #endregion

        #region 系统Cpu
        /// <summary>
        /// 系统CPU配置
        /// </summary>
        /// <param name="searchkey"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public ActionResult DefaultSystemCpu(string searchkey, string index)
        {
            //1.从中心库获取表：DataBaseServerConfigures中所有服务器名称
            var serverNameList = MongoDBHelper.Select<DataBaseServerConfigures>(MongoConnType.Center);
            var searchKu = new List<string>();
            foreach (var item in serverNameList)
            {
                searchKu.Add(item.DataBaseName);
            }
            //2.将所有服务器名称传递给页面，选择之后当成参数传递过来
            //3.修改连接字符串，查找服务器与数据库，表为固定的
            //4.传值
            var baseSearch = new BaseSearchModel();
            baseSearch.SearchKu = searchKu;
            ViewData["baseSearchModel"] = baseSearch;

            //此为多表数据，初始化无法确定加载哪一个，故此传NULL
            List<SystemCpu> systemPingList = null;

            //用于筛选路径
            var searchAddress = new Dictionary<string, string>();
            searchAddress.Add("Controller", RouteData.Values["controller"].ToString());
            searchAddress.Add("Action", RouteData.Values["action"].ToString() + "Page");
            ViewData["searchAddress"] = searchAddress;
            return PartialView(systemPingList);
        }
        public ActionResult DefaultSystemCpuPage(string searchkey, string index)
        {
            #region 筛选
            string searchKeyWord = Request["SearchKeyWord"] ?? "";
            string searchDateTimeStart = Request["SearchDateTimeStart"] ?? "";
            string searchDateTimeEnd = Request["SearchDateTimeEnd"] ?? "";
            string SearchKu = Request["SearchKu"] ?? "";
            #endregion

            //mongodb筛选条件
            List<IMongoQuery> queryList = new List<IMongoQuery>();
            IMongoQuery query = null;

            if (string.IsNullOrWhiteSpace(index))
            {
                index = "1";
            }
            if (!string.IsNullOrWhiteSpace(searchKeyWord))
            {
                //queryList.Add(Query<SystemCpu>.Matches(item => item.CpuUsageRate, searchKeyWord));
            }
            if (!string.IsNullOrWhiteSpace(searchDateTimeStart))
            {
                queryList.Add(Query<SystemCpu>.GTE(item => item.DateTimeNow, Convert.ToDateTime(searchDateTimeStart)));
            }
            if (!string.IsNullOrWhiteSpace(searchDateTimeEnd))
            {
                queryList.Add(Query<SystemCpu>.LTE(item => item.DateTimeNow, Convert.ToDateTime(searchDateTimeStart)));
            }
            if (queryList.Count > 0)
            {
                query = Query.And(queryList.ToArray());
            }
            string projectKu = string.Empty;
            if (!string.IsNullOrWhiteSpace(SearchKu))
            {
                projectKu = SearchKu.Replace('.', '-');
            }
            else
            {
                //此为多表数据，初始化无法确定加载哪一个，故此传NULL
                List<SystemCpu> systemPingList = null;
                return PartialView(systemPingList);
            }
            var mongoOther = MongoDBHelper.MongoConnModelList.FirstOrDefault(item => item.MongoConnType == MongoConnType.Other);
            if (mongoOther != null)
            {
                MongoDBHelper.MongoConnModelList.Remove(mongoOther);
            }
            MongoDBHelper.MongoConnModelList.Add(new MongoConnModel() { MongoConnType = MongoConnType.Other, MongoConnStr = "mongodb://" + MongoDBUserName + ":" + MongoDBUserPassWord + "@" + SearchKu + ":27017", MongoDatabase = "project" + projectKu });
            //mongodb排序
            var sortBy = SortBy<SystemCpu>.Descending(item => item.DateTimeNow);

            //mongodb获取全部记录数
            long recordCount;
            MongoDBHelper.Select<SystemCpu>(query, out recordCount, MongoConnType.Other);

            BasePageModel page = new BasePageModel() { SearchKeyWord = searchkey, ActionName = RouteData.Values["action"].ToString(), CurrentIndex = Int32.Parse(index), TotalCount = recordCount };
            var systemCpuList = MongoDBHelper.Select<SystemCpu>(page.PageSize, page.CurrentIndex, query, sortBy, MongoConnType.Other);
            ViewData["pagemodel"] = page;
            return PartialView(systemCpuList);
        }
        #endregion

        #region 系统硬盘
        /// <summary>
        /// 系统硬盘配置
        /// </summary>
        /// <param name="searchkey"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public ActionResult DefaultSystemHdd(string searchkey, string index)
        {
            //1.从中心库获取表：DataBaseServerConfigures中所有服务器名称
            var serverNameList = MongoDBHelper.Select<DataBaseServerConfigures>(MongoConnType.Center);
            var searchKu = new List<string>();
            foreach (var item in serverNameList)
            {
                searchKu.Add(item.DataBaseName);
            }
            //2.将所有服务器名称传递给页面，选择之后当成参数传递过来
            //3.修改连接字符串，查找服务器与数据库，表为固定的
            //4.传值
            var baseSearch = new BaseSearchModel();
            baseSearch.SearchKu = searchKu;
            ViewData["baseSearchModel"] = baseSearch;

            //此为多表数据，初始化无法确定加载哪一个，故此传NULL
            List<SystemHdd> systemPingList = null;

            //用于筛选路径
            var searchAddress = new Dictionary<string, string>();
            searchAddress.Add("Controller", RouteData.Values["controller"].ToString());
            searchAddress.Add("Action", RouteData.Values["action"].ToString() + "Page");
            ViewData["searchAddress"] = searchAddress;
            return PartialView(systemPingList);
        }
        public ActionResult DefaultSystemHddPage(string searchkey, string index)
        {
            #region 筛选
            string searchKeyWord = Request["SearchKeyWord"] ?? "";
            string searchDateTimeStart = Request["SearchDateTimeStart"] ?? "";
            string searchDateTimeEnd = Request["SearchDateTimeEnd"] ?? "";
            string SearchKu = Request["SearchKu"] ?? "";
            #endregion

            //mongodb筛选条件
            List<IMongoQuery> queryList = new List<IMongoQuery>();
            IMongoQuery query = null;

            if (string.IsNullOrWhiteSpace(index))
            {
                index = "1";
            }
            if (!string.IsNullOrWhiteSpace(searchKeyWord))
            {
                //queryList.Add(Query<SystemHdd>.Matches(item => item.CpuUsageRate, searchKeyWord));
            }
            if (!string.IsNullOrWhiteSpace(searchDateTimeStart))
            {
                queryList.Add(Query<SystemHdd>.GTE(item => item.DateTimeNow, Convert.ToDateTime(searchDateTimeStart)));
            }
            if (!string.IsNullOrWhiteSpace(searchDateTimeEnd))
            {
                queryList.Add(Query<SystemHdd>.LTE(item => item.DateTimeNow, Convert.ToDateTime(searchDateTimeStart)));
            }
            if (queryList.Count > 0)
            {
                query = Query.And(queryList.ToArray());
            }
            string projectKu = string.Empty;
            if (!string.IsNullOrWhiteSpace(SearchKu))
            {
                projectKu = SearchKu.Replace('.', '-');
            }
            else
            {
                //此为多表数据，初始化无法确定加载哪一个，故此传NULL
                List<SystemHdd> systemPingList = null;
                return PartialView(systemPingList);
            }
            var mongoOther = MongoDBHelper.MongoConnModelList.FirstOrDefault(item => item.MongoConnType == MongoConnType.Other);
            if (mongoOther != null)
            {
                MongoDBHelper.MongoConnModelList.Remove(mongoOther);
            }
            MongoDBHelper.MongoConnModelList.Add(new MongoConnModel() { MongoConnType = MongoConnType.Other, MongoConnStr = "mongodb://" + MongoDBUserName + ":" + MongoDBUserPassWord + "@" + SearchKu + ":27017", MongoDatabase = "project" + projectKu });
            //mongodb排序
            var sortBy = SortBy<SystemHdd>.Descending(item => item.DateTimeNow);

            //mongodb获取全部记录数
            long recordCount;
            MongoDBHelper.Select<SystemHdd>(query, out recordCount, MongoConnType.Other);

            BasePageModel page = new BasePageModel() { SearchKeyWord = searchkey, ActionName = RouteData.Values["action"].ToString(), CurrentIndex = Int32.Parse(index), TotalCount = recordCount };
            var systemCpuList = MongoDBHelper.Select<SystemHdd>(page.PageSize, page.CurrentIndex, query, sortBy, MongoConnType.Other);
            ViewData["pagemodel"] = page;
            return PartialView(systemCpuList);
        }
        #endregion

        #region 系统网络
        /// <summary>
        /// 系统网络配置
        /// </summary>
        /// <param name="searchkey"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public ActionResult DefaultSystemInternet(string searchkey, string index)
        {
            //1.从中心库获取表：DataBaseServerConfigures中所有服务器名称
            var serverNameList = MongoDBHelper.Select<DataBaseServerConfigures>(MongoConnType.Center);
            var searchKu = new List<string>();
            foreach (var item in serverNameList)
            {
                searchKu.Add(item.DataBaseName);
            }
            //2.将所有服务器名称传递给页面，选择之后当成参数传递过来
            //3.修改连接字符串，查找服务器与数据库，表为固定的
            //4.传值
            var baseSearch = new BaseSearchModel();
            baseSearch.SearchKu = searchKu;
            ViewData["baseSearchModel"] = baseSearch;

            //此为多表数据，初始化无法确定加载哪一个，故此传NULL
            List<SystemInternet> systemPingList = null;

            //用于筛选路径
            var searchAddress = new Dictionary<string, string>();
            searchAddress.Add("Controller", RouteData.Values["controller"].ToString());
            searchAddress.Add("Action", RouteData.Values["action"].ToString() + "Page");
            ViewData["searchAddress"] = searchAddress;
            return PartialView(systemPingList);
        }
        public ActionResult DefaultSystemInternetPage(string searchkey, string index)
        {
            #region 筛选
            string searchKeyWord = Request["SearchKeyWord"] ?? "";
            string searchDateTimeStart = Request["SearchDateTimeStart"] ?? "";
            string searchDateTimeEnd = Request["SearchDateTimeEnd"] ?? "";
            string SearchKu = Request["SearchKu"] ?? "";
            #endregion

            //mongodb筛选条件
            List<IMongoQuery> queryList = new List<IMongoQuery>();
            IMongoQuery query = null;

            if (string.IsNullOrWhiteSpace(index))
            {
                index = "1";
            }
            if (!string.IsNullOrWhiteSpace(searchKeyWord))
            {
                //queryList.Add(Query<SystemInternet>.Matches(item => item.CpuUsageRate, searchKeyWord));
            }
            if (!string.IsNullOrWhiteSpace(searchDateTimeStart))
            {
                queryList.Add(Query<SystemInternet>.GTE(item => item.DateTimeNow, Convert.ToDateTime(searchDateTimeStart)));
            }
            if (!string.IsNullOrWhiteSpace(searchDateTimeEnd))
            {
                queryList.Add(Query<SystemInternet>.LTE(item => item.DateTimeNow, Convert.ToDateTime(searchDateTimeStart)));
            }
            if (queryList.Count > 0)
            {
                query = Query.And(queryList.ToArray());
            }
            string projectKu = string.Empty;
            if (!string.IsNullOrWhiteSpace(SearchKu))
            {
                projectKu = SearchKu.Replace('.', '-');
            }
            else
            {
                //此为多表数据，初始化无法确定加载哪一个，故此传NULL
                List<SystemInternet> systemPingList = null;
                return PartialView(systemPingList);
            }
            var mongoOther = MongoDBHelper.MongoConnModelList.FirstOrDefault(item => item.MongoConnType == MongoConnType.Other);
            if (mongoOther != null)
            {
                MongoDBHelper.MongoConnModelList.Remove(mongoOther);
            }
            MongoDBHelper.MongoConnModelList.Add(new MongoConnModel() { MongoConnType = MongoConnType.Other, MongoConnStr = "mongodb://" + MongoDBUserName + ":" + MongoDBUserPassWord + "@" + SearchKu + ":27017", MongoDatabase = "project" + projectKu });
            //mongodb排序
            var sortBy = SortBy<SystemInternet>.Descending(item => item.DateTimeNow);

            //mongodb获取全部记录数
            long recordCount;
            MongoDBHelper.Select<SystemInternet>(query, out recordCount, MongoConnType.Other);

            BasePageModel page = new BasePageModel() { SearchKeyWord = searchkey, ActionName = RouteData.Values["action"].ToString(), CurrentIndex = Int32.Parse(index), TotalCount = recordCount };
            var systemCpuList = MongoDBHelper.Select<SystemInternet>(page.PageSize, page.CurrentIndex, query, sortBy, MongoConnType.Other);
            ViewData["pagemodel"] = page;
            return PartialView(systemCpuList);
        }
        #endregion

        #region 系统内存
        /// <summary>
        /// 系统内存配置
        /// </summary>
        /// <param name="searchkey"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public ActionResult DefaultSystemMemories(string searchkey, string index)
        {
            //1.从中心库获取表：DataBaseServerConfigures中所有服务器名称
            var serverNameList = MongoDBHelper.Select<DataBaseServerConfigures>(MongoConnType.Center);
            var searchKu = new List<string>();
            foreach (var item in serverNameList)
            {
                searchKu.Add(item.DataBaseName);
            }
            //2.将所有服务器名称传递给页面，选择之后当成参数传递过来
            //3.修改连接字符串，查找服务器与数据库，表为固定的
            //4.传值
            var baseSearch = new BaseSearchModel();
            baseSearch.SearchKu = searchKu;
            ViewData["baseSearchModel"] = baseSearch;

            //此为多表数据，初始化无法确定加载哪一个，故此传NULL
            List<SystemMemories> systemPingList = null;

            //用于筛选路径
            var searchAddress = new Dictionary<string, string>();
            searchAddress.Add("Controller", RouteData.Values["controller"].ToString());
            searchAddress.Add("Action", RouteData.Values["action"].ToString() + "Page");
            ViewData["searchAddress"] = searchAddress;
            return PartialView(systemPingList);
        }
        public ActionResult DefaultSystemMemoriesPage(string searchkey, string index)
        {
            #region 筛选
            string searchKeyWord = Request["SearchKeyWord"] ?? "";
            string searchDateTimeStart = Request["SearchDateTimeStart"] ?? "";
            string searchDateTimeEnd = Request["SearchDateTimeEnd"] ?? "";
            string SearchKu = Request["SearchKu"] ?? "";
            #endregion

            //mongodb筛选条件
            List<IMongoQuery> queryList = new List<IMongoQuery>();
            IMongoQuery query = null;

            if (string.IsNullOrWhiteSpace(index))
            {
                index = "1";
            }
            if (!string.IsNullOrWhiteSpace(searchKeyWord))
            {
                //queryList.Add(Query<SystemInternet>.Matches(item => item.CpuUsageRate, searchKeyWord));
            }
            if (!string.IsNullOrWhiteSpace(searchDateTimeStart))
            {
                queryList.Add(Query<SystemMemories>.GTE(item => item.DateTimeNow, Convert.ToDateTime(searchDateTimeStart)));
            }
            if (!string.IsNullOrWhiteSpace(searchDateTimeEnd))
            {
                queryList.Add(Query<SystemMemories>.LTE(item => item.DateTimeNow, Convert.ToDateTime(searchDateTimeStart)));
            }
            if (queryList.Count > 0)
            {
                query = Query.And(queryList.ToArray());
            }
            string projectKu = string.Empty;
            if (!string.IsNullOrWhiteSpace(SearchKu))
            {
                projectKu = SearchKu.Replace('.', '-');
            }
            else
            {
                //此为多表数据，初始化无法确定加载哪一个，故此传NULL
                List<SystemMemories> systemPingList = null;
                return PartialView(systemPingList);
            }
            var mongoOther = MongoDBHelper.MongoConnModelList.FirstOrDefault(item => item.MongoConnType == MongoConnType.Other);
            if (mongoOther != null)
            {
                MongoDBHelper.MongoConnModelList.Remove(mongoOther);
            }
            MongoDBHelper.MongoConnModelList.Add(new MongoConnModel() { MongoConnType = MongoConnType.Other, MongoConnStr = "mongodb://" + MongoDBUserName + ":" + MongoDBUserPassWord + "@" + SearchKu + ":27017", MongoDatabase = "project" + projectKu });
            //mongodb排序
            var sortBy = SortBy<SystemMemories>.Descending(item => item.DateTimeNow);

            //mongodb获取全部记录数
            long recordCount;
            MongoDBHelper.Select<SystemMemories>(query, out recordCount, MongoConnType.Other);

            BasePageModel page = new BasePageModel() { SearchKeyWord = searchkey, ActionName = RouteData.Values["action"].ToString(), CurrentIndex = Int32.Parse(index), TotalCount = recordCount };
            var systemCpuList = MongoDBHelper.Select<SystemMemories>(page.PageSize, page.CurrentIndex, query, sortBy, MongoConnType.Other);
            ViewData["pagemodel"] = page;
            return PartialView(systemCpuList);
        }
        #endregion

        #region 进程Cpu
        /// <summary>
        /// 进程Cpu配置
        /// </summary>
        /// <param name="searchkey"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public ActionResult DefaultProcessCpu(string searchkey, string index)
        {
            //1.从中心库获取表：DataBaseServerConfigures中所有服务器名称
            var serverNameList = MongoDBHelper.Select<DataBaseServerConfigures>(MongoConnType.Center);
            var searchKu = new List<string>();
            foreach (var item in serverNameList)
            {
                searchKu.Add(item.DataBaseName);
            }
            //2.将所有服务器名称传递给页面，选择之后当成参数传递过来
            //3.修改连接字符串，查找服务器与数据库，表为固定的
            //4.传值
            var baseSearch = new BaseSearchModel();
            baseSearch.SearchKu = searchKu;
            ViewData["baseSearchModel"] = baseSearch;

            //此为多表数据，初始化无法确定加载哪一个，故此传NULL
            List<ProcessCpu> systemPingList = null;

            //用于筛选路径
            var searchAddress = new Dictionary<string, string>();
            searchAddress.Add("Controller", RouteData.Values["controller"].ToString());
            searchAddress.Add("Action", RouteData.Values["action"].ToString() + "Page");
            ViewData["searchAddress"] = searchAddress;
            return PartialView(systemPingList);
        }
        public ActionResult DefaultProcessCpuPage(string searchkey, string index)
        {
            #region 筛选
            string searchKeyWord = Request["SearchKeyWord"] ?? "";
            string searchDateTimeStart = Request["SearchDateTimeStart"] ?? "";
            string searchDateTimeEnd = Request["SearchDateTimeEnd"] ?? "";
            string SearchKu = Request["SearchKu"] ?? "";
            #endregion

            //mongodb筛选条件
            List<IMongoQuery> queryList = new List<IMongoQuery>();
            IMongoQuery query = null;

            if (string.IsNullOrWhiteSpace(index))
            {
                index = "1";
            }
            if (!string.IsNullOrWhiteSpace(searchKeyWord))
            {
                //queryList.Add(Query<ProcessCpu>.Matches(item => item.CpuUsageRate, searchKeyWord));
            }
            if (!string.IsNullOrWhiteSpace(searchDateTimeStart))
            {
                queryList.Add(Query<ProcessCpu>.GTE(item => item.DateTimeNow, Convert.ToDateTime(searchDateTimeStart)));
            }
            if (!string.IsNullOrWhiteSpace(searchDateTimeEnd))
            {
                queryList.Add(Query<ProcessCpu>.LTE(item => item.DateTimeNow, Convert.ToDateTime(searchDateTimeStart)));
            }
            if (queryList.Count > 0)
            {
                query = Query.And(queryList.ToArray());
            }
            string projectKu = string.Empty;
            if (!string.IsNullOrWhiteSpace(SearchKu))
            {
                projectKu = SearchKu.Replace('.', '-');
            }
            else
            {
                //此为多表数据，初始化无法确定加载哪一个，故此传NULL
                List<ProcessCpu> systemPingList = null;
                return PartialView(systemPingList);
            }
            var mongoOther = MongoDBHelper.MongoConnModelList.FirstOrDefault(item => item.MongoConnType == MongoConnType.Other);
            if (mongoOther != null)
            {
                MongoDBHelper.MongoConnModelList.Remove(mongoOther);
            }
            MongoDBHelper.MongoConnModelList.Add(new MongoConnModel() { MongoConnType = MongoConnType.Other, MongoConnStr = "mongodb://" + MongoDBUserName + ":" + MongoDBUserPassWord + "@" + SearchKu + ":27017", MongoDatabase = "project" + projectKu });
            //mongodb排序
            var sortBy = SortBy<ProcessCpu>.Descending(item => item.DateTimeNow);

            //mongodb获取全部记录数
            long recordCount;
            MongoDBHelper.Select<ProcessCpu>(query, out recordCount, MongoConnType.Other);

            BasePageModel page = new BasePageModel() { SearchKeyWord = searchkey, ActionName = RouteData.Values["action"].ToString(), CurrentIndex = Int32.Parse(index), TotalCount = recordCount };
            var systemCpuList = MongoDBHelper.Select<ProcessCpu>(page.PageSize, page.CurrentIndex, query, sortBy, MongoConnType.Other);
            ViewData["pagemodel"] = page;
            return PartialView(systemCpuList);
        }
        #endregion

        #region 进程网络
        /// <summary>
        /// 进程网络配置
        /// </summary>
        /// <param name="searchkey"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public ActionResult DefaultProcessInternet(string searchkey, string index)
        {
            //1.从中心库获取表：DataBaseServerConfigures中所有服务器名称
            var serverNameList = MongoDBHelper.Select<DataBaseServerConfigures>(MongoConnType.Center);
            var searchKu = new List<string>();
            foreach (var item in serverNameList)
            {
                searchKu.Add(item.DataBaseName);
            }
            //2.将所有服务器名称传递给页面，选择之后当成参数传递过来
            //3.修改连接字符串，查找服务器与数据库，表为固定的
            //4.传值
            var baseSearch = new BaseSearchModel();
            baseSearch.SearchKu = searchKu;
            ViewData["baseSearchModel"] = baseSearch;

            //此为多表数据，初始化无法确定加载哪一个，故此传NULL
            List<ProcessInternet> systemPingList = null;

            //用于筛选路径
            var searchAddress = new Dictionary<string, string>();
            searchAddress.Add("Controller", RouteData.Values["controller"].ToString());
            searchAddress.Add("Action", RouteData.Values["action"].ToString() + "Page");
            ViewData["searchAddress"] = searchAddress;
            return PartialView(systemPingList);
        }
        public ActionResult DefaultProcessInternetPage(string searchkey, string index)
        {
            #region 筛选
            string searchKeyWord = Request["SearchKeyWord"] ?? "";
            string searchDateTimeStart = Request["SearchDateTimeStart"] ?? "";
            string searchDateTimeEnd = Request["SearchDateTimeEnd"] ?? "";
            string SearchKu = Request["SearchKu"] ?? "";
            #endregion

            //mongodb筛选条件
            List<IMongoQuery> queryList = new List<IMongoQuery>();
            IMongoQuery query = null;

            if (string.IsNullOrWhiteSpace(index))
            {
                index = "1";
            }
            if (!string.IsNullOrWhiteSpace(searchKeyWord))
            {
                //queryList.Add(Query<ProcessInternet>.Matches(item => item.CpuUsageRate, searchKeyWord));
            }
            if (!string.IsNullOrWhiteSpace(searchDateTimeStart))
            {
                queryList.Add(Query<ProcessInternet>.GTE(item => item.DateTimeNow, Convert.ToDateTime(searchDateTimeStart)));
            }
            if (!string.IsNullOrWhiteSpace(searchDateTimeEnd))
            {
                queryList.Add(Query<ProcessInternet>.LTE(item => item.DateTimeNow, Convert.ToDateTime(searchDateTimeStart)));
            }
            if (queryList.Count > 0)
            {
                query = Query.And(queryList.ToArray());
            }
            string projectKu = string.Empty;
            if (!string.IsNullOrWhiteSpace(SearchKu))
            {
                projectKu = SearchKu.Replace('.', '-');
            }
            else
            {
                //此为多表数据，初始化无法确定加载哪一个，故此传NULL
                List<ProcessInternet> systemPingList = null;
                return PartialView(systemPingList);
            }
            var mongoOther = MongoDBHelper.MongoConnModelList.FirstOrDefault(item => item.MongoConnType == MongoConnType.Other);
            if (mongoOther != null)
            {
                MongoDBHelper.MongoConnModelList.Remove(mongoOther);
            }
            MongoDBHelper.MongoConnModelList.Add(new MongoConnModel() { MongoConnType = MongoConnType.Other, MongoConnStr = "mongodb://" + MongoDBUserName + ":" + MongoDBUserPassWord + "@" + SearchKu + ":27017", MongoDatabase = "project" + projectKu });
            //mongodb排序
            var sortBy = SortBy<ProcessInternet>.Descending(item => item.DateTimeNow);

            //mongodb获取全部记录数
            long recordCount;
            MongoDBHelper.Select<ProcessInternet>(query, out recordCount, MongoConnType.Other);

            BasePageModel page = new BasePageModel() { SearchKeyWord = searchkey, ActionName = RouteData.Values["action"].ToString(), CurrentIndex = Int32.Parse(index), TotalCount = recordCount };
            var systemCpuList = MongoDBHelper.Select<ProcessInternet>(page.PageSize, page.CurrentIndex, query, sortBy, MongoConnType.Other);
            ViewData["pagemodel"] = page;
            return PartialView(systemCpuList);
        }
        #endregion

        #region 进程内存
        /// <summary>
        /// 进程内存配置
        /// </summary>
        /// <param name="searchkey"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public ActionResult DefaultProcessMemories(string searchkey, string index)
        {
            //1.从中心库获取表：DataBaseServerConfigures中所有服务器名称
            var serverNameList = MongoDBHelper.Select<DataBaseServerConfigures>(MongoConnType.Center);
            var searchKu = new List<string>();
            foreach (var item in serverNameList)
            {
                searchKu.Add(item.DataBaseName);
            }
            //2.将所有服务器名称传递给页面，选择之后当成参数传递过来
            //3.修改连接字符串，查找服务器与数据库，表为固定的
            //4.传值
            var baseSearch = new BaseSearchModel();
            baseSearch.SearchKu = searchKu;
            ViewData["baseSearchModel"] = baseSearch;

            //此为多表数据，初始化无法确定加载哪一个，故此传NULL
            List<ProcessMemories> systemPingList = null;

            //用于筛选路径
            var searchAddress = new Dictionary<string, string>();
            searchAddress.Add("Controller", RouteData.Values["controller"].ToString());
            searchAddress.Add("Action", RouteData.Values["action"].ToString() + "Page");
            ViewData["searchAddress"] = searchAddress;
            return PartialView(systemPingList);
        }
        public ActionResult DefaultProcessMemoriesPage(string searchkey, string index)
        {
            #region 筛选
            string searchKeyWord = Request["SearchKeyWord"] ?? "";
            string searchDateTimeStart = Request["SearchDateTimeStart"] ?? "";
            string searchDateTimeEnd = Request["SearchDateTimeEnd"] ?? "";
            string SearchKu = Request["SearchKu"] ?? "";
            #endregion

            //mongodb筛选条件
            List<IMongoQuery> queryList = new List<IMongoQuery>();
            IMongoQuery query = null;

            if (string.IsNullOrWhiteSpace(index))
            {
                index = "1";
            }
            if (!string.IsNullOrWhiteSpace(searchKeyWord))
            {
                //queryList.Add(Query<ProcessMemories>.Matches(item => item.CpuUsageRate, searchKeyWord));
            }
            if (!string.IsNullOrWhiteSpace(searchDateTimeStart))
            {
                queryList.Add(Query<ProcessMemories>.GTE(item => item.DateTimeNow, Convert.ToDateTime(searchDateTimeStart)));
            }
            if (!string.IsNullOrWhiteSpace(searchDateTimeEnd))
            {
                queryList.Add(Query<ProcessMemories>.LTE(item => item.DateTimeNow, Convert.ToDateTime(searchDateTimeStart)));
            }
            if (queryList.Count > 0)
            {
                query = Query.And(queryList.ToArray());
            }
            string projectKu = string.Empty;
            if (!string.IsNullOrWhiteSpace(SearchKu))
            {
                projectKu = SearchKu.Replace('.', '-');
            }
            else
            {
                //此为多表数据，初始化无法确定加载哪一个，故此传NULL
                List<ProcessMemories> systemPingList = null;
                return PartialView(systemPingList);
            }
            var mongoOther = MongoDBHelper.MongoConnModelList.FirstOrDefault(item => item.MongoConnType == MongoConnType.Other);
            if (mongoOther != null)
            {
                MongoDBHelper.MongoConnModelList.Remove(mongoOther);
            }
            MongoDBHelper.MongoConnModelList.Add(new MongoConnModel() { MongoConnType = MongoConnType.Other, MongoConnStr = "mongodb://" + MongoDBUserName + ":" + MongoDBUserPassWord + "@" + SearchKu + ":27017", MongoDatabase = "project" + projectKu });
            //mongodb排序
            var sortBy = SortBy<ProcessMemories>.Descending(item => item.DateTimeNow);

            //mongodb获取全部记录数
            long recordCount;
            MongoDBHelper.Select<ProcessMemories>(query, out recordCount, MongoConnType.Other);

            BasePageModel page = new BasePageModel() { SearchKeyWord = searchkey, ActionName = RouteData.Values["action"].ToString(), CurrentIndex = Int32.Parse(index), TotalCount = recordCount };
            var systemCpuList = MongoDBHelper.Select<ProcessMemories>(page.PageSize, page.CurrentIndex, query, sortBy, MongoConnType.Other);
            ViewData["pagemodel"] = page;
            return PartialView(systemCpuList);
        }
        #endregion

        #region 综合查询
        public ActionResult SystenPingLineHour()
        {
            //Thread.Sleep(300);
            #region 筛选
            string searchKeyWord = Request["SearchKeyWord"] ?? "";
            string searchDateTimeStart = Request["SearchDateTimeStart"] ?? "";
            string searchDateTimeEnd = Request["SearchDateTimeEnd"] ?? "";
            string SearchBiao = Request["SearchBiao"] ?? "";
            #endregion

            //mongodb筛选条件
            List<IMongoQuery> queryList = new List<IMongoQuery>();

            IMongoQuery query = null;

            if (!string.IsNullOrWhiteSpace(searchKeyWord))
            {
                queryList.Add(Query<SystemPing>.Matches(item => item.PingIP, searchKeyWord));
            }
            if (!string.IsNullOrWhiteSpace(searchDateTimeStart))
            {
                queryList.Add(Query<SystemPing>.GTE(item => item.DateTimeNow, Convert.ToDateTime(searchDateTimeStart)));
            }
            else
            {
                //最新一个小时的数据
                queryList.Add(Query<SystemPing>.GTE(item => item.DateTimeNow, Convert.ToDateTime(DateTime.Now.AddHours(-1))));
            }
            if (!string.IsNullOrWhiteSpace(searchDateTimeEnd))
            {
                queryList.Add(Query<SystemPing>.LTE(item => item.DateTimeNow, Convert.ToDateTime(searchDateTimeEnd)));
            }
            if (queryList.Count > 0)
            {
                query = Query.And(queryList.ToArray());
            }
            //先判断此表是否存在,如不存在返回空
            if (!MongoDBHelper.Exist("SystemPing" + SearchBiao, MongoConnType.Center))
            {
                return Content("");
            }

            //mongodb排序
            var sortBy = SortBy<SystemPing>.Descending(item => item.DateTimeNow);

            var systemPingList = MongoDBHelper.Select<SystemPing>("SystemPing" + SearchBiao, query, sortBy, MongoConnType.Center);

            DateTime startHours = DateTime.Now.AddHours(-1);

            //var dataListStatus = (from q in systemPingList
            //                      group q by new { Status = q.Status } into dsGroup
            //                      select new
            //                      {
            //                          Status = dsGroup.Key.Status
            //                      });

            List<ValueText> listTrue = new List<ValueText>();
            List<ValueText> listFalse = new List<ValueText>();
            for (DateTime date = startHours; date <= DateTime.Now; )
            {
                //bool trueflag = false;
                foreach (var item in systemPingList)
                {
                    if (item.DateTimeNow.Year == date.Year && item.DateTimeNow.Month == date.Month && item.DateTimeNow.Day == date.Day && (item.DateTimeNow.Hour + 8) == date.Hour && item.DateTimeNow.Minute == date.Minute && item.DateTimeNow.Second == date.Second)
                    {

                        //trueflag = true;
                        if (item.Status == "请求成功")
                        {
                            listTrue.Add(new ValueText
                            {
                                Text = date.ToString("yyyy年MM月dd日 HH时mm分ss秒"),
                                Value = item.PingDate
                            });
                            listFalse.Add(new ValueText
                            {
                                Text = date.ToString("yyyy年MM月dd日 HH时mm分ss秒"),
                                Value = 0
                            });
                        }
                        else
                        {
                            listTrue.Add(new ValueText
                            {
                                Text = date.ToString("yyyy年MM月dd日 HH时mm分ss秒"),
                                Value = 0
                            });
                            listFalse.Add(new ValueText
                            {
                                Text = date.ToString("yyyy年MM月dd日 HH时mm分ss秒"),
                                Value = 1
                            });

                        }
                        continue;
                    }
                }
                //if (!trueflag)
                //{
                //    listTrue.Add(new ValueText
                //    {
                //        Text = date.ToString("yyyy-MM-dd HH:mm:ss"),
                //        Value = 0
                //    });
                //}
                date = date.AddSeconds(1);
            }
            var optionJson = new echartsOption()
            {
                title = new title()
                {
                    text = "近一小时系统\nPing数量",
                    y = "top",
                    x = "left"
                },
                tooltip = new tooltip()
                {
                    trigger = "axis"
                },
                legend = new legend()
                {
                    data = new string[] { "请求成功时间(ms)", "请求失败(次数)" }
                },
                grid = new grid()
                {
                    left = "3%",
                    right = "4%",
                    bottom = "3%",
                    top = "8%",
                    containLabel = true
                },
                toolbox = new toolbox() { feature = new feature() { } },
                xAxis = new xAxis()
                {
                    type = "category",
                    boundaryGap = false,
                    data = listTrue.Select(item => item.Text).ToArray()
                },
                yAxis = new yAxis()
                {
                    type = "value"
                },
                series = new series[]
                    {
                        new series()
                        { 
                            name="请求成功时间(ms)", type="line", data=listTrue.Select(item=>item.Value).ToArray()
                        },
                        new series()
                        { 
                            name="请求失败(次数)", type="line", data=listFalse.Select(item=>item.Value).ToArray()
                        }
                    }
            };
            var stringJson = JsonConvert.SerializeObject(optionJson, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            return Content(stringJson);
        }
        public ActionResult SystemCpuLineHour()
        {
            Thread.Sleep(300);

            #region Request筛选
            string searchKeyWord = Request["SearchKeyWord"] ?? "";
            string searchDateTimeStart = Request["SearchDateTimeStart"] ?? "";
            string searchDateTimeEnd = Request["SearchDateTimeEnd"] ?? "";
            string SearchKu = Request["SearchKu"] ?? "";
            #endregion

            #region MongoDb筛选

            //mongodb筛选条件
            List<IMongoQuery> queryList = new List<IMongoQuery>();

            IMongoQuery query = null;

            if (!string.IsNullOrWhiteSpace(searchKeyWord))
            {
                queryList.Add(Query<SystemCpu>.Matches(item => item._id, searchKeyWord));
            }
            if (!string.IsNullOrWhiteSpace(searchDateTimeStart))
            {
                queryList.Add(Query<SystemCpu>.GTE(item => item.DateTimeNow, Convert.ToDateTime(searchDateTimeStart)));
            }
            else
            {
                queryList.Add(Query<SystemCpu>.GTE(item => item.DateTimeNow, Convert.ToDateTime(DateTime.Now.AddHours(-1))));
            }
            if (!string.IsNullOrWhiteSpace(searchDateTimeEnd))
            {
                queryList.Add(Query<SystemCpu>.LTE(item => item.DateTimeNow, Convert.ToDateTime(searchDateTimeStart)));
            }
            if (queryList.Count > 0)
            {
                query = Query.And(queryList.ToArray());
            }
            #endregion

            #region SearchKu为空判断与替换

            string projectKu = string.Empty;
            if (!string.IsNullOrWhiteSpace(SearchKu))
            {
                projectKu = SearchKu.Replace('.', '-');
            }
            else
            {
                return Content("");
            }
            #endregion

            #region 数据库链接修改

            var mongoOther = MongoDBHelper.MongoConnModelList.FirstOrDefault(item => item.MongoConnType == MongoConnType.Other);
            if (mongoOther != null)
            {
                MongoDBHelper.MongoConnModelList.Remove(mongoOther);
            }
            MongoDBHelper.MongoConnModelList.Add(new MongoConnModel() { MongoConnType = MongoConnType.Other, MongoConnStr = "mongodb://" + MongoDBUserName + ":" + MongoDBUserPassWord + "@" + SearchKu + ":27017", MongoDatabase = "project" + projectKu });

            #endregion

            #region 查询
            var List = MongoDBHelper.Select<SystemCpu>(query, MongoConnType.Other);

            DateTime startDate = DateTime.Now.AddHours(-1);

            //var dataList = (from q in List
            //                group q by new { Month = q.DateTimeNow.Month, Year = q.DateTimeNow.Year, Day = q.DateTimeNow.Day } into dsGroup
            //                select new
            //                {
            //                    Year = dsGroup.Key.Year,
            //                    Month = dsGroup.Key.Month,
            //                    Day = dsGroup.Key.Day,
            //                    Count = dsGroup.Count(),
            //                }).OrderBy(u => u.Year);

            List<ValueText> list = new List<ValueText>();
            for (DateTime date = startDate; date <= DateTime.Now; )
            {
                //bool flag = false;
                foreach (var item in List)
                {
                    if (item.DateTimeNow.Year == date.Year && item.DateTimeNow.Month == date.Month && item.DateTimeNow.Day == date.Day && (item.DateTimeNow.Hour + 8) == date.Hour && item.DateTimeNow.Minute == date.Minute && item.DateTimeNow.Second == date.Second)
                    {
                        //flag = true;
                        list.Add(new ValueText
                        {
                            Text = date.ToString("yyyy年MM月dd日 HH时mm分:ss秒"),
                            Value = item.CpuUsageRate
                        });
                    }
                }
                //if (!flag)
                //{
                //    list.Add(new ValueText
                //    {
                //        Text = date.Year + "-" + date.Month + "-" + date.Day,
                //        Value = 0
                //    });
                //}
                date = date.AddSeconds(1);
            }

            var optionJson = new echartsOption()
            {
                title = new title()
                {
                    text = "近一小时系统\nCpu使用率",
                    y = "top",
                    x = "left"
                },
                tooltip = new tooltip()
                {
                    trigger = "axis"
                },
                legend = new legend()
                {
                    data = new string[] { "系统Cpu使用率(%)" }
                },
                grid = new grid()
                {
                    left = "3%",
                    right = "4%",
                    bottom = "3%",
                    top = "8%",
                    containLabel = true
                },
                toolbox = new toolbox() { feature = new feature() { } },
                xAxis = new xAxis()
                {
                    type = "category",
                    boundaryGap = false,
                    data = list.Select(item => item.Text).ToArray()
                },
                yAxis = new yAxis()
                {
                    type = "value"
                },
                series = new series[]
                    {
                        new series()
                        { 
                            name="系统Cpu使用率(%)", type="line",  stack="总量", data=list.Select(item=>item.Value).ToArray()
                        }
                    }
            };
            #endregion
            var stringJson = JsonConvert.SerializeObject(optionJson, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            return Content(stringJson);
        }
        public ActionResult SystemHddBar()
        {
            #region Request筛选
            string searchKeyWord = Request["SearchKeyWord"] ?? "";
            string searchDateTimeStart = Request["SearchDateTimeStart"] ?? "";
            string searchDateTimeEnd = Request["SearchDateTimeEnd"] ?? "";
            string SearchKu = Request["SearchKu"] ?? "";
            #endregion

            #region MongoDb筛选

            //mongodb筛选条件
            List<IMongoQuery> queryList = new List<IMongoQuery>();

            //获取最近十天的数据
            queryList.Add(Query<SystemHdd>.GTE(item => item.DateTimeNow, Convert.ToDateTime(DateTime.Now.AddDays(-30))));

            IMongoQuery query = null;

            if (!string.IsNullOrWhiteSpace(searchKeyWord))
            {
                queryList.Add(Query<SystemHdd>.Matches(item => item.HddName, searchKeyWord));
            }
            if (!string.IsNullOrWhiteSpace(searchDateTimeStart))
            {
                //queryList.Add(Query<SystemHdd>.GTE(item => item.DateTimeNow, Convert.ToDateTime(searchDateTimeStart)));
            }
            if (!string.IsNullOrWhiteSpace(searchDateTimeEnd))
            {
                //queryList.Add(Query<SystemHdd>.LTE(item => item.DateTimeNow, Convert.ToDateTime(searchDateTimeStart)));
            }
            if (queryList.Count > 0)
            {
                query = Query.And(queryList.ToArray());
            }
            #endregion

            #region SearchKu为空判断与替换

            string projectKu = string.Empty;
            if (!string.IsNullOrWhiteSpace(SearchKu))
            {
                projectKu = SearchKu.Replace('.', '-');
            }
            else
            {
                return Content("");
            }
            #endregion

            #region 数据库链接修改

            var mongoOther = MongoDBHelper.MongoConnModelList.FirstOrDefault(item => item.MongoConnType == MongoConnType.Other);
            if (mongoOther != null)
            {
                MongoDBHelper.MongoConnModelList.Remove(mongoOther);
            }
            MongoDBHelper.MongoConnModelList.Add(new MongoConnModel() { MongoConnType = MongoConnType.Other, MongoConnStr = "mongodb://" + MongoDBUserName + ":" + MongoDBUserPassWord + "@" + SearchKu + ":27017", MongoDatabase = "project" + projectKu });

            #endregion

            #region 查询

            //mongodb排序
            var sortBy = SortBy<SystemHdd>.Descending(item => item.DateTimeNow);

            var List = MongoDBHelper.Select<SystemHdd>(query, sortBy, MongoConnType.Other);

            //根据分组，获取有几个盘符
            var hddNames = (from q in List
                            group q by new { Name = q.HddName } into dsGroup
                            select new
                            {
                                Name = dsGroup.Key.Name
                            });
            if (hddNames == null || hddNames.Count() <= 0)
            {
                return Content("");
            }
            else
            {
                var hddModelCharts = new List<SystemHdd>();
                foreach (var item in hddNames)
                {
                    var hddModel = List.Where(hn => item.Name == hn.HddName).OrderByDescending(hn => hn.DateTimeNow).FirstOrDefault();
                    if (hddModel == null)
                        continue;
                    else
                        hddModelCharts.Add(new SystemHdd() { HddName = hddModel.HddName, AllSpace = hddModel.AllSpace, SurplusSpace = hddModel.SurplusSpace, UseSpace = hddModel.UseSpace });
                }

                var optionJson = new echartsOption()
                {
                    title = new title()
                    {
                        text = "最新系统\n硬盘使用情况",
                        y = "top",
                        x = "left"
                    },
                    tooltip = new tooltip() { },
                    legend = new legend()
                    {
                        data = new string[] { "总量(GB)", "已使用(GB)", "剩余(GB)" }
                    },
                    xAxis = new xAxis()
                    {
                        data = hddModelCharts.Select(item => item.HddName).ToArray()
                    },
                    yAxis = new yAxis() { },
                    series = new series[]
                {
                    new series()
                    { 
                        name="总量(GB)", type="bar", data= hddModelCharts.Select(item=>item.AllSpace).ToArray()
                    },
                    new series()
                    { 
                        name="已使用(GB)", type="bar", data= hddModelCharts.Select(item=>item.UseSpace).ToArray()
                    },
                    new series()
                    { 
                        name="剩余(GB)", type="bar", data= hddModelCharts.Select(item=>item.SurplusSpace).ToArray()
                    }
                }
                };
            #endregion

                var stringJson = JsonConvert.SerializeObject(optionJson, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                return Content(stringJson);
            }
        }
        public ActionResult SystemMemoriesHour()
        {
            Thread.Sleep(300);

            #region Request筛选
            string searchKeyWord = Request["SearchKeyWord"] ?? "";
            string searchDateTimeStart = Request["SearchDateTimeStart"] ?? "";
            string searchDateTimeEnd = Request["SearchDateTimeEnd"] ?? "";
            string SearchKu = Request["SearchKu"] ?? "";
            #endregion

            #region MongoDb筛选

            //mongodb筛选条件
            List<IMongoQuery> queryList = new List<IMongoQuery>();

            IMongoQuery query = null;

            if (!string.IsNullOrWhiteSpace(searchKeyWord))
            {
                queryList.Add(Query<SystemMemories>.Matches(item => item._id, searchKeyWord));
            }
            if (!string.IsNullOrWhiteSpace(searchDateTimeStart))
            {
                queryList.Add(Query<SystemMemories>.GTE(item => item.DateTimeNow, Convert.ToDateTime(searchDateTimeStart)));
            }
            else
            {
                queryList.Add(Query<SystemMemories>.GTE(item => item.DateTimeNow, Convert.ToDateTime(DateTime.Now.AddHours(-1))));
            }
            if (!string.IsNullOrWhiteSpace(searchDateTimeEnd))
            {
                queryList.Add(Query<SystemMemories>.LTE(item => item.DateTimeNow, Convert.ToDateTime(searchDateTimeStart)));
            }
            if (queryList.Count > 0)
            {
                query = Query.And(queryList.ToArray());
            }
            #endregion

            #region SearchKu为空判断与替换

            string projectKu = string.Empty;
            if (!string.IsNullOrWhiteSpace(SearchKu))
            {
                projectKu = SearchKu.Replace('.', '-');
            }
            else
            {
                return Content("");
            }
            #endregion

            #region 数据库链接修改

            var mongoOther = MongoDBHelper.MongoConnModelList.FirstOrDefault(item => item.MongoConnType == MongoConnType.Other);
            if (mongoOther != null)
            {
                MongoDBHelper.MongoConnModelList.Remove(mongoOther);
            }
            MongoDBHelper.MongoConnModelList.Add(new MongoConnModel() { MongoConnType = MongoConnType.Other, MongoConnStr = "mongodb://" + MongoDBUserName + ":" + MongoDBUserPassWord + "@" + SearchKu + ":27017", MongoDatabase = "project" + projectKu });

            #endregion

            #region 查询
            var List = MongoDBHelper.Select<SystemMemories>(query, MongoConnType.Other);

            DateTime startDate = DateTime.Now.AddHours(-1);

            //var dataList = (from q in List
            //                group q by new { Month = q.DateTimeNow.Month, Year = q.DateTimeNow.Year, Day = q.DateTimeNow.Day } into dsGroup
            //                select new
            //                {
            //                    Year = dsGroup.Key.Year,
            //                    Month = dsGroup.Key.Month,
            //                    Day = dsGroup.Key.Day,
            //                    Count = dsGroup.Count(),
            //                }).OrderBy(u => u.Year);

            List<ValueText> list = new List<ValueText>();
            for (DateTime date = startDate; date <= DateTime.Now; )
            {
                //bool flag = false;
                foreach (var item in List)
                {
                    if (item.DateTimeNow.Year == date.Year && item.DateTimeNow.Month == date.Month && item.DateTimeNow.Day == date.Day && (item.DateTimeNow.Hour + 8) == date.Hour && item.DateTimeNow.Minute == date.Minute && item.DateTimeNow.Second == date.Second)
                    {
                        //flag = true;
                        list.Add(new ValueText
                        {
                            Text = date.ToString("yyyy年MM月dd日 HH时mm分:ss秒"),
                            Value = item.MemoryUsageRate,
                            ViceValue = (double)item.MemoryUsageGB
                        });
                    }
                }
                //if (!flag)
                //{
                //    list.Add(new ValueText
                //    {
                //        Text = date.Year + "-" + date.Month + "-" + date.Day,
                //        Value = 0
                //    });
                //}
                date = date.AddSeconds(1);
            }

            var optionJson = new echartsOption()
            {
                title = new title()
                {
                    text = "近一小时系统\n内存使用率",
                    y = "top",
                    x = "left"
                },
                tooltip = new tooltip()
                {
                    trigger = "axis"
                },
                legend = new legend()
                {
                    data = new string[] { "系统内存使用率(%)", "系统内存使用(GB)" }
                },
                grid = new grid()
                {
                    left = "3%",
                    right = "4%",
                    bottom = "3%",
                    top = "8%",
                    containLabel = true
                },
                toolbox = new toolbox() { feature = new feature() { } },
                xAxis = new xAxis()
                {
                    type = "category",
                    boundaryGap = false,
                    data = list.Select(item => item.Text).ToArray()
                },
                yAxis = new yAxis()
                {
                    type = "value"
                },
                series = new series[]
                    {
                        new series()
                        { 
                            name="系统内存使用率(%)", type="line",  data=list.Select(item=>item.Value).ToArray()
                        },
                        new series()
                        { 
                            name="系统内存使用(GB)", type="line",   data=list.Select(item=>item.ViceValue).ToArray()
                        }
                    }
            };
            #endregion
            var stringJson = JsonConvert.SerializeObject(optionJson, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            return Content(stringJson);
        }
        public ActionResult ProcessMemoriesHour()
        {
            Thread.Sleep(300);

            #region Request筛选
            string searchKeyWord = Request["SearchKeyWord"] ?? "";
            string searchDateTimeStart = Request["SearchDateTimeStart"] ?? "";
            string searchDateTimeEnd = Request["SearchDateTimeEnd"] ?? "";
            string SearchKu = Request["SearchKu"] ?? "";
            #endregion

            #region MongoDb筛选

            //mongodb筛选条件
            List<IMongoQuery> queryList = new List<IMongoQuery>();

            IMongoQuery query = null;

            if (!string.IsNullOrWhiteSpace(searchKeyWord))
            {
                queryList.Add(Query<SystemMemories>.Matches(item => item._id, searchKeyWord));
            }
            if (!string.IsNullOrWhiteSpace(searchDateTimeStart))
            {
                queryList.Add(Query<SystemMemories>.GTE(item => item.DateTimeNow, Convert.ToDateTime(searchDateTimeStart)));
            }
            else
            {
                queryList.Add(Query<SystemMemories>.GTE(item => item.DateTimeNow, Convert.ToDateTime(DateTime.Now.AddHours(-1))));
            }
            if (!string.IsNullOrWhiteSpace(searchDateTimeEnd))
            {
                queryList.Add(Query<SystemMemories>.LTE(item => item.DateTimeNow, Convert.ToDateTime(searchDateTimeStart)));
            }
            if (queryList.Count > 0)
            {
                query = Query.And(queryList.ToArray());
            }
            #endregion

            #region SearchKu为空判断与替换

            string projectKu = string.Empty;
            if (!string.IsNullOrWhiteSpace(SearchKu))
            {
                projectKu = SearchKu.Replace('.', '-');
            }
            else
            {
                return Content("");
            }
            #endregion

            #region 数据库链接修改

            var mongoOther = MongoDBHelper.MongoConnModelList.FirstOrDefault(item => item.MongoConnType == MongoConnType.Other);
            if (mongoOther != null)
            {
                MongoDBHelper.MongoConnModelList.Remove(mongoOther);
            }
            MongoDBHelper.MongoConnModelList.Add(new MongoConnModel() { MongoConnType = MongoConnType.Other, MongoConnStr = "mongodb://" + MongoDBUserName + ":" + MongoDBUserPassWord + "@" + SearchKu + ":27017", MongoDatabase = "project" + projectKu });

            #endregion

            #region 查询
            var List = MongoDBHelper.Select<ProcessMemories>(query, MongoConnType.Other);
            //根据分组，获取有几个盘符
            var hddNames = (from q in List
                            group q by new { Name = q.ProcessName } into dsGroup
                            select new
                            {
                                Name = dsGroup.Key.Name
                            });
            //用于存储series对象，后期转为数组
            var seriesList = new List<series>();

            DateTime startDate = DateTime.Now.AddHours(-1);

            List<ValueText> list = new List<ValueText>();
            foreach (var hddName in hddNames)
            {
                var valueList = List.Where(l => l.ProcessName == hddName.Name);
                for (DateTime date = startDate; date <= DateTime.Now; )
                {
                    foreach (var item in valueList)
                    {
                        if (item.DateTimeNow.Year == date.Year && item.DateTimeNow.Month == date.Month && item.DateTimeNow.Day == date.Day && (item.DateTimeNow.Hour + 8) == date.Hour && item.DateTimeNow.Minute == date.Minute && item.DateTimeNow.Second == date.Second)
                        {
                            list.Add(new ValueText
                            {
                                Text = date.ToString("yyyy年MM月dd日 HH时mm分:ss秒"),
                                Value = (double)item.ProcessSpecialUseKB
                            });
                        }
                    }
                    date = date.AddSeconds(1);
                }
                seriesList.Add(new series()
                {
                    name = hddName.Name + "内存使用情况(KB)",
                    type = "line",
                    data = list.Select(item => item.Value).ToArray()

                });
            }
            var optionJson = new echartsOption()
            {
                title = new title()
                {
                    text = "近一小时进程\n内存使用情况",
                    y = "top",
                    x = "left"
                },
                tooltip = new tooltip()
                {
                    trigger = "axis"
                },
                legend = new legend()
                {
                    data = seriesList.Select(item => item.name).ToArray<string>()
                },
                grid = new grid()
                {
                    left = "3%",
                    right = "4%",
                    bottom = "3%",
                    top = "8%",
                    containLabel = true
                },
                toolbox = new toolbox() { feature = new feature() { } },
                xAxis = new xAxis()
                {
                    type = "category",
                    boundaryGap = false,
                    data = list.Select(item => item.Text).ToArray()
                },
                yAxis = new yAxis()
                {
                    type = "value"
                },
                series = seriesList.ToArray()
            };
            #endregion
            var stringJson = JsonConvert.SerializeObject(optionJson, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            return Content(stringJson);
        }
        public ActionResult ProcessCpuHour()
        {
            Thread.Sleep(300);

            #region Request筛选
            string searchKeyWord = Request["SearchKeyWord"] ?? "";
            string searchDateTimeStart = Request["SearchDateTimeStart"] ?? "";
            string searchDateTimeEnd = Request["SearchDateTimeEnd"] ?? "";
            string SearchKu = Request["SearchKu"] ?? "";
            #endregion

            #region MongoDb筛选

            //mongodb筛选条件
            List<IMongoQuery> queryList = new List<IMongoQuery>();

            IMongoQuery query = null;

            if (!string.IsNullOrWhiteSpace(searchKeyWord))
            {
                queryList.Add(Query<ProcessCpu>.Matches(item => item._id, searchKeyWord));
            }
            if (!string.IsNullOrWhiteSpace(searchDateTimeStart))
            {
                queryList.Add(Query<ProcessCpu>.GTE(item => item.DateTimeNow, Convert.ToDateTime(searchDateTimeStart)));
            }
            else
            {
                queryList.Add(Query<ProcessCpu>.GTE(item => item.DateTimeNow, Convert.ToDateTime(DateTime.Now.AddHours(-1))));
            }
            if (!string.IsNullOrWhiteSpace(searchDateTimeEnd))
            {
                queryList.Add(Query<ProcessCpu>.LTE(item => item.DateTimeNow, Convert.ToDateTime(searchDateTimeStart)));
            }
            if (queryList.Count > 0)
            {
                query = Query.And(queryList.ToArray());
            }
            #endregion

            #region SearchKu为空判断与替换

            string projectKu = string.Empty;
            if (!string.IsNullOrWhiteSpace(SearchKu))
            {
                projectKu = SearchKu.Replace('.', '-');
            }
            else
            {
                return Content("");
            }
            #endregion

            #region 数据库链接修改

            var mongoOther = MongoDBHelper.MongoConnModelList.FirstOrDefault(item => item.MongoConnType == MongoConnType.Other);
            if (mongoOther != null)
            {
                MongoDBHelper.MongoConnModelList.Remove(mongoOther);
            }
            MongoDBHelper.MongoConnModelList.Add(new MongoConnModel() { MongoConnType = MongoConnType.Other, MongoConnStr = "mongodb://" + MongoDBUserName + ":" + MongoDBUserPassWord + "@" + SearchKu + ":27017", MongoDatabase = "project" + projectKu });

            #endregion

            #region 查询
            var List = MongoDBHelper.Select<ProcessCpu>(query, MongoConnType.Other);
            //根据分组，获取有几个盘符
            var hddNames = (from q in List
                            group q by new { Name = q.ProcessName } into dsGroup
                            select new
                            {
                                Name = dsGroup.Key.Name
                            });
            //用于存储series对象，后期转为数组
            var seriesList = new List<series>();

            DateTime startDate = DateTime.Now.AddHours(-1);

            List<ValueText> list = new List<ValueText>();
            foreach (var hddName in hddNames)
            {
                var valueList = List.Where(l => l.ProcessName == hddName.Name);
                for (DateTime date = startDate; date <= DateTime.Now; )
                {
                    foreach (var item in valueList)
                    {
                        if (item.DateTimeNow.Year == date.Year && item.DateTimeNow.Month == date.Month && item.DateTimeNow.Day == date.Day && (item.DateTimeNow.Hour + 8) == date.Hour && item.DateTimeNow.Minute == date.Minute && item.DateTimeNow.Second == date.Second)
                        {
                            list.Add(new ValueText
                            {
                                Text = date.ToString("yyyy年MM月dd日 HH时mm分:ss秒"),
                                Value = (double)item.ProcessUsageRate
                            });
                        }
                    }
                    date = date.AddSeconds(1);
                }
                seriesList.Add(new series()
                {
                    name = hddName.Name + " Cpu使用情况",
                    type = "line",
                    data = list.Select(item => item.Value).ToArray()

                });
            }
            var optionJson = new echartsOption()
            {
                title = new title()
                {
                    text = "近一小时进程\nCpu使用情况",
                    y = "top",
                    x = "left"
                },
                tooltip = new tooltip()
                {
                    trigger = "axis"
                },
                legend = new legend()
                {
                    data = seriesList.Select(item => item.name).ToArray<string>()
                },
                grid = new grid()
                {
                    left = "3%",
                    right = "4%",
                    bottom = "3%",
                    top = "8%",
                    containLabel = true
                },
                toolbox = new toolbox() { feature = new feature() { } },
                xAxis = new xAxis()
                {
                    type = "category",
                    boundaryGap = false,
                    data = list.Select(item => item.Text).ToArray()
                },
                yAxis = new yAxis()
                {
                    type = "value"
                },
                series = seriesList.ToArray()
            };
            #endregion
            var stringJson = JsonConvert.SerializeObject(optionJson, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            return Content(stringJson);
        }
        public ActionResult SystemInternetHour()
        {
            Thread.Sleep(300);

            #region Request筛选
            string searchKeyWord = Request["SearchKeyWord"] ?? "";
            string searchDateTimeStart = Request["SearchDateTimeStart"] ?? "";
            string searchDateTimeEnd = Request["SearchDateTimeEnd"] ?? "";
            string SearchKu = Request["SearchKu"] ?? "";
            #endregion

            #region MongoDb筛选

            //mongodb筛选条件
            List<IMongoQuery> queryList = new List<IMongoQuery>();

            IMongoQuery query = null;

            if (!string.IsNullOrWhiteSpace(searchKeyWord))
            {
                queryList.Add(Query<SystemMemories>.Matches(item => item._id, searchKeyWord));
            }
            if (!string.IsNullOrWhiteSpace(searchDateTimeStart))
            {
                queryList.Add(Query<SystemMemories>.GTE(item => item.DateTimeNow, Convert.ToDateTime(searchDateTimeStart)));
            }
            else
            {
                queryList.Add(Query<SystemMemories>.GTE(item => item.DateTimeNow, Convert.ToDateTime(DateTime.Now.AddHours(-1))));
            }
            if (!string.IsNullOrWhiteSpace(searchDateTimeEnd))
            {
                queryList.Add(Query<SystemMemories>.LTE(item => item.DateTimeNow, Convert.ToDateTime(searchDateTimeStart)));
            }
            if (queryList.Count > 0)
            {
                query = Query.And(queryList.ToArray());
            }
            #endregion

            #region SearchKu为空判断与替换

            string projectKu = string.Empty;
            if (!string.IsNullOrWhiteSpace(SearchKu))
            {
                projectKu = SearchKu.Replace('.', '-');
            }
            else
            {
                return Content("");
            }
            #endregion

            #region 数据库链接修改

            var mongoOther = MongoDBHelper.MongoConnModelList.FirstOrDefault(item => item.MongoConnType == MongoConnType.Other);
            if (mongoOther != null)
            {
                MongoDBHelper.MongoConnModelList.Remove(mongoOther);
            }
            MongoDBHelper.MongoConnModelList.Add(new MongoConnModel() { MongoConnType = MongoConnType.Other, MongoConnStr = "mongodb://" + MongoDBUserName + ":" + MongoDBUserPassWord + "@" + SearchKu + ":27017", MongoDatabase = "project" + projectKu });

            #endregion

            #region 查询
            var List = MongoDBHelper.Select<SystemInternet>(query, MongoConnType.Other);

            DateTime startDate = DateTime.Now.AddHours(-1);

            List<ValueText> list = new List<ValueText>();
            for (DateTime date = startDate; date <= DateTime.Now; )
            {
                foreach (var item in List)
                {
                    if (item.DateTimeNow.Year == date.Year && item.DateTimeNow.Month == date.Month && item.DateTimeNow.Day == date.Day && (item.DateTimeNow.Hour + 8) == date.Hour && item.DateTimeNow.Minute == date.Minute && item.DateTimeNow.Second == date.Second)
                    {
                        list.Add(new ValueText
                        {
                            Text = date.ToString("yyyy年MM月dd日 HH时mm分:ss秒"),
                            Value = double.Parse(item.UploadByte),
                            ViceValue = double.Parse(item.DownloadByte)
                        });
                    }
                }
                date = date.AddSeconds(1);
            }

            var optionJson = new echartsOption()
            {
                title = new title()
                {
                    text = "近一小时系统\n网络使用情况",
                    y = "top",
                    x = "left"
                },
                tooltip = new tooltip()
                {
                    trigger = "axis"
                },
                legend = new legend()
                {
                    data = new string[] { "网络上传(Kbps)", "网络下载(Kbps)" }
                },
                grid = new grid()
                {
                    left = "3%",
                    right = "4%",
                    bottom = "3%",
                    top = "8%",
                    containLabel = true
                },
                toolbox = new toolbox() { feature = new feature() { } },
                xAxis = new xAxis()
                {
                    type = "category",
                    boundaryGap = false,
                    data = list.Select(item => item.Text).ToArray()
                },
                yAxis = new yAxis()
                {
                    type = "value"
                },
                series = new series[]
                    {
                        new series()
                        { 
                            name="网络上传(Kbps)", type="line",  data=list.Select(item=>item.Value).ToArray()
                        },
                        new series()
                        { 
                            name="网络下载(Kbps)", type="line",   data=list.Select(item=>item.ViceValue).ToArray()
                        }
                    }
            };
            #endregion
            var stringJson = JsonConvert.SerializeObject(optionJson, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            return Content(stringJson);
        }
        #endregion
    }
}
