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
using Newtonsoft.Json.Linq;
using TLZ.MongoDB;
using TLZ.OldSite.DB.MongoDB.Model;
using TLZ.OldSite.WebSite.Admin.Models;

namespace TLZ.OldSite.WebSite.Admin.Controllers
{
    [UserValidate]
    public class StatisticsController : Controller
    {
        readonly string MongoDBUserName = ConfigurationManager.AppSettings["MongoDBUserName"].ToString();
        readonly string MongoDBUserPassWord = ConfigurationManager.AppSettings["MongoDBUserPassWord"].ToString();

        #region 活动统计

        /// <summary>
        /// 活动统计
        /// </summary>
        /// <returns></returns>
        public ActionResult ActivityConfigures()
        {
            #region 筛选条件
            var baseSearch = new BaseSearchModel();
            ViewData["baseSearchModel"] = baseSearch;
            #endregion

            //用于筛选路径
            var searchAddress = new Dictionary<string, string>();
            searchAddress.Add("Controller", RouteData.Values["controller"].ToString());
            searchAddress.Add("Action", RouteData.Values["action"].ToString() + "Echarts");
            ViewData["searchAddress"] = searchAddress;
            return PartialView();
        }
        public ActionResult ActivityConfiguresEcharts()
        {
            return PartialView();
        }
        public ActionResult ActivityConfiguresBar()
        {
            var List = MongoDBHelper.Select<ActivityConfigures>(MongoConnType.Center).GroupBy(item => item.ActivityName).Select(item => new { name = item.Key, count = item.Count() });
            var optionJson = new echartsOption()
            {
                title = new title()
                {
                    text = "活动配置"
                },
                tooltip = new tooltip() { },
                legend = new legend()
                {
                    data = new string[] { "总数" }
                },
                xAxis = new xAxis()
                {
                    data = List.Select(item => item.name).ToArray()
                },
                yAxis = new yAxis() { },
                series = new series[]
                {
                    new series()
                    { 
                        name="总数", type="bar", data=List.Select(item => (double)item.count).ToArray()
                    }
                }
            };
            var stringJson = JsonConvert.SerializeObject(optionJson, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            return Content(stringJson);
        }

        #endregion

        #region 服务器统计

        /// <summary>
        /// 服务器统计
        /// </summary>
        /// <returns></returns>
        public ActionResult DataBaseServerConfigures()
        {
            #region 筛选条件
            var baseSearch = new BaseSearchModel();
            ViewData["baseSearchModel"] = baseSearch;
            #endregion

            //用于筛选路径
            var searchAddress = new Dictionary<string, string>();
            searchAddress.Add("Controller", RouteData.Values["controller"].ToString());
            searchAddress.Add("Action", RouteData.Values["action"].ToString() + "Echarts");
            ViewData["searchAddress"] = searchAddress;
            return PartialView();
        }
        public ActionResult DataBaseServerConfiguresEcharts()
        {
            return PartialView();
        }
        public ActionResult DataBaseServerConfiguresBar()
        {
            var List = MongoDBHelper.Select<DataBaseServerConfigures>(MongoConnType.Center).GroupBy(item => item.DataBaseName).Select(item => new { name = item.Key, count = item.Count() });
            var optionJson = new echartsOption()
            {
                title = new title()
                {
                    text = "服务器配置"
                },
                tooltip = new tooltip() { },
                legend = new legend()
                {
                    data = new string[] { "总数" }
                },
                xAxis = new xAxis()
                {
                    data = List.Select(item => item.name).ToArray()
                },
                yAxis = new yAxis() { },
                series = new series[]
                {
                    new series()
                    { 
                        name="总数", type="bar", data=List.Select(item => (double)item.count).ToArray()
                    }
                }
            };
            var stringJson = JsonConvert.SerializeObject(optionJson, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            return Content(stringJson);
        }

        #endregion

        #region 服务器Ping统计
        /// <summary>
        /// 服务器Ping统计
        /// </summary>
        /// <returns></returns>
        public ActionResult SystenPing()
        {
            #region 筛选条件
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
            #endregion

            //用于筛选路径
            var searchAddress = new Dictionary<string, string>();
            searchAddress.Add("Controller", RouteData.Values["controller"].ToString());
            searchAddress.Add("Action", RouteData.Values["action"].ToString() + "Echarts");
            ViewData["searchAddress"] = searchAddress;
            return PartialView();
        }
        public ActionResult SystenPingEcharts()
        {
            return PartialView();
        }

        #region 柱状图
        //public ActionResult SystenPingBar()
        //{
        //    #region 筛选
        //    string searchKeyWord = Request["SearchKeyWord"] ?? "";
        //    string searchDateTimeStart = Request["SearchDateTimeStart"] ?? "";
        //    string searchDateTimeEnd = Request["SearchDateTimeEnd"] ?? "";
        //    string SearchBiao = Request["SearchBiao"] ?? "";
        //    #endregion

        //    //mongodb筛选条件
        //    List<IMongoQuery> queryList = new List<IMongoQuery>();

        //    IMongoQuery query = null;

        //    if (!string.IsNullOrWhiteSpace(searchKeyWord))
        //    {
        //        queryList.Add(Query<SystemPing>.Matches(item => item.PingIP, searchKeyWord));
        //    }
        //    if (!string.IsNullOrWhiteSpace(searchDateTimeStart))
        //    {
        //        queryList.Add(Query<SystemPing>.GTE(item => item.DateTimeNow, Convert.ToDateTime(searchDateTimeStart)));
        //    }
        //    else
        //    {
        //        //获取最近三十天的数据
        //        queryList.Add(Query<SystemPing>.GTE(item => item.DateTimeNow, Convert.ToDateTime(DateTime.Now.AddDays(-30))));
        //    }
        //    if (!string.IsNullOrWhiteSpace(searchDateTimeEnd))
        //    {
        //        queryList.Add(Query<SystemPing>.LTE(item => item.DateTimeNow, Convert.ToDateTime(searchDateTimeStart)));
        //    }
        //    if (queryList.Count > 0)
        //    {
        //        query = Query.And(queryList.ToArray());
        //    }
        //    //先判断此表是否存在,如不存在返回空
        //    if (!MongoDBHelper.Exist("SystemPing" + SearchBiao, MongoConnType.Center))
        //    {
        //        return Content("");
        //    }
        //    var systemPingList = MongoDBHelper.Select<SystemPing>("SystemPing" + SearchBiao, query, MongoConnType.Center);

        //    DateTime startMonth = DateTime.Now.AddDays(-30);

        //    var dataList = (from q in systemPingList
        //                    group q by new { Month = q.DateTimeNow.Month, Year = q.DateTimeNow.Year, Day = q.DateTimeNow.Day } into dsGroup
        //                    select new
        //                    {
        //                        Year = dsGroup.Key.Year,
        //                        Month = dsGroup.Key.Month,
        //                        Day = dsGroup.Key.Day,
        //                        Count = dsGroup.Count(),
        //                    }).OrderBy(u => u.Year);

        //    List<ValueText> list = new List<ValueText>();
        //    for (DateTime date = startMonth; date <= DateTime.Now; )
        //    {
        //        bool flag = false;
        //        foreach (var item in dataList)
        //        {
        //            if (item.Year == date.Year && item.Month == date.Month && item.Day == date.Day)
        //            {
        //                flag = true;
        //                list.Add(new ValueText
        //                {
        //                    Text = item.Year + "-" + item.Month + "-" + item.Day,
        //                    Value = item.Count
        //                });
        //            }
        //        }
        //        if (!flag)
        //        {
        //            list.Add(new ValueText
        //            {
        //                Text = date.Year + "-" + date.Month + "-" + date.Day,
        //                Value = 0
        //            });
        //        }
        //        date = date.AddDays(1);
        //    }

        //    var optionJson = new echartsOption()
        //    {
        //        title = new title()
        //        {
        //            text = "近三十天系统Ping数量"
        //        },
        //        tooltip = new tooltip() { },
        //        legend = new legend()
        //        {
        //            data = new string[] { "系统Ping值" }
        //        },
        //        xAxis = new xAxis()
        //        {
        //            data = list.Select(item => item.Text).ToArray()
        //        },
        //        yAxis = new yAxis() { },
        //        series = new series[]
        //        {
        //            new series()
        //            { 
        //                name="系统Ping值", type="bar", data= list.Select(item=>item.Value).ToArray()
        //            }
        //        }
        //    };
        //    var stringJson = JsonConvert.SerializeObject(optionJson, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        //    return Content(stringJson);
        //}
        #endregion

        public ActionResult SystenPingLineMinute()
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
                //最新一分钟的数据
                queryList.Add(Query<SystemPing>.GTE(item => item.DateTimeNow, Convert.ToDateTime(DateTime.Now.AddMinutes(-1))));
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

            DateTime startHours = DateTime.Now.AddMinutes(-1);

            List<ValueText> listTrue = new List<ValueText>();
            List<ValueText> listFalse = new List<ValueText>();
            for (DateTime date = startHours; date <= DateTime.Now; )
            {
                foreach (var item in systemPingList)
                {
                    if (item.DateTimeNow.Year == date.Year && item.DateTimeNow.Month == date.Month && item.DateTimeNow.Day == date.Day && (item.DateTimeNow.Hour + 8) == date.Hour && item.DateTimeNow.Minute == date.Minute && item.DateTimeNow.Second == date.Second)
                    {
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
                date = date.AddSeconds(1);
            }
            var optionJson = new echartsOption()
            {
                title = new title()
                {
                    text = "近一分钟系统Ping数量"
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

            List<ValueText> listTrue = new List<ValueText>();
            List<ValueText> listFalse = new List<ValueText>();
            for (DateTime date = startHours; date <= DateTime.Now; )
            {
                foreach (var item in systemPingList)
                {
                    if (item.DateTimeNow.Year == date.Year && item.DateTimeNow.Month == date.Month && item.DateTimeNow.Day == date.Day && (item.DateTimeNow.Hour + 8) == date.Hour && item.DateTimeNow.Minute == date.Minute && item.DateTimeNow.Second == date.Second)
                    {
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
                date = date.AddSeconds(1);
            }
            var optionJson = new echartsOption()
            {
                title = new title()
                {
                    text = "近一小时系统Ping数量"
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

        public ActionResult SystenPingLineDay()
        {
            //Thread.Sleep(600);
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
                //最新一天的数据
                queryList.Add(Query<SystemPing>.GTE(item => item.DateTimeNow, Convert.ToDateTime(DateTime.Now.AddDays(-1))));
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

            DateTime startDay = DateTime.Now.AddDays(-1);

            List<ValueText> listTrue = new List<ValueText>();
            List<ValueText> listFalse = new List<ValueText>();
            for (DateTime date = startDay; date <= DateTime.Now; )
            {
                double pingTimeCount = 0;
                int pingCount = 0;
                int pingFail = 0;
                foreach (var item in systemPingList)
                {
                    if (item.DateTimeNow.Year == date.Year && item.DateTimeNow.Month == date.Month && item.DateTimeNow.Day == date.Day && (item.DateTimeNow.Hour + 8) == date.Hour && item.DateTimeNow.Minute == date.Minute)
                    {
                        if (item.Status == "请求成功")
                        {
                            pingCount++;
                            pingTimeCount += item.PingDate;
                        }
                        else
                        {
                            pingFail++;
                        }
                    }
                }
                listTrue.Add(new ValueText
                {
                    Text = date.ToString("yyyy年MM月dd日 HH时mm分"),
                    Value = pingCount != 0 ? (pingTimeCount / pingCount) : 0
                });
                listFalse.Add(new ValueText
                {
                    Text = date.ToString("yyyy年MM月dd日 HH时mm分"),
                    Value = pingFail
                });
                date = date.AddMinutes(1);
            }
            var optionJson = new echartsOption()
            {
                title = new title()
                {
                    text = "近一天系统Ping数量"
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

        public ActionResult SystenPingLineWeek()
        {
            //Thread.Sleep(900);
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
                //最新一周的数据
                queryList.Add(Query<SystemPing>.GTE(item => item.DateTimeNow, Convert.ToDateTime(DateTime.Now.AddDays(-7))));
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

            DateTime startDay = DateTime.Now.AddDays(-7);

            //var dataListStatus = (from q in systemPingList
            //                      group q by new { Status = q.Status } into dsGroup
            //                      select new
            //                      {
            //                          Status = dsGroup.Key.Status
            //                      });

            List<ValueText> listTrue = new List<ValueText>();
            List<ValueText> listFalse = new List<ValueText>();
            for (DateTime date = startDay; date <= DateTime.Now; )
            {
                //bool trueflag = false;
                double pingTimeCount = 0;
                int pingCount = 0;
                int pingFail = 0;
                foreach (var item in systemPingList)
                {

                    if (item.DateTimeNow.Year == date.Year && item.DateTimeNow.Month == date.Month && item.DateTimeNow.Day == date.Day && (item.DateTimeNow.Hour + 8) == date.Hour)
                    {
                        //trueflag = true;
                        if (item.Status == "请求成功")
                        {
                            pingCount++;
                            pingTimeCount += item.PingDate;
                        }
                        else
                        {
                            pingFail++;
                        }
                    }
                }
                listTrue.Add(new ValueText
                {
                    Text = date.ToString("yyyy年MM月dd日 HH时"),
                    Value = pingCount != 0 ? (pingTimeCount / pingCount) : 0
                });
                listFalse.Add(new ValueText
                {
                    Text = date.ToString("yyyy年MM月dd日 HH时"),
                    Value = pingFail
                });
                //if (!trueflag)
                //{
                //    listTrue.Add(new ValueText
                //    {
                //        Text = date.ToString("yyyy-MM-dd HH:mm:ss"),
                //        Value = 0
                //    });
                //}
                date = date.AddHours(1);
            }
            var optionJson = new echartsOption()
            {
                title = new title()
                {
                    text = "近一周系统Ping数量"
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

        #endregion

        #region 系统Cpu统计
        public ActionResult SystemCpu()
        {
            #region 筛选条件
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
            #endregion

            //用于筛选路径
            var searchAddress = new Dictionary<string, string>();
            searchAddress.Add("Controller", RouteData.Values["controller"].ToString());
            searchAddress.Add("Action", RouteData.Values["action"].ToString() + "Echarts");
            ViewData["searchAddress"] = searchAddress;
            return PartialView();
        }

        public ActionResult SystemCpuEcharts()
        {
            return PartialView();
        }

        public ActionResult SystemCpuLineMinute()
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
                queryList.Add(Query<SystemCpu>.GTE(item => item.DateTimeNow, Convert.ToDateTime(DateTime.Now.AddMinutes(-1))));
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

            DateTime startDate = DateTime.Now.AddMinutes(-1);

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
                    text = "近一分钟系统Cpu使用率"
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
                    text = "近一小时系统Cpu使用率"
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

        public ActionResult SystemCpuLineDay()
        {
            Thread.Sleep(600);

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
                queryList.Add(Query<SystemCpu>.GTE(item => item.DateTimeNow, Convert.ToDateTime(DateTime.Now.AddDays(-1))));
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

            DateTime startDate = DateTime.Now.AddDays(-1);

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
                double pingLvCount = 0;
                int pingCount = 0;
                foreach (var item in List)
                {

                    if (item.DateTimeNow.Year == date.Year && item.DateTimeNow.Month == date.Month && item.DateTimeNow.Day == date.Day && (item.DateTimeNow.Hour + 8) == date.Hour && item.DateTimeNow.Minute == date.Minute)
                    {
                        //trueflag = true;

                        pingCount++;
                        pingLvCount += item.CpuUsageRate;

                    }
                }
                list.Add(new ValueText
                {
                    Text = date.ToString("yyyy年MM月dd日 HH时mm分"),
                    Value = pingCount != 0 ? Math.Round(Math.Round((pingLvCount / pingCount), 1), 1) : 0
                });
                //if (!trueflag)
                //{
                //    listTrue.Add(new ValueText
                //    {
                //        Text = date.ToString("yyyy-MM-dd HH:mm:ss"),
                //        Value = 0
                //    });
                //}
                date = date.AddMinutes(1);
            }

            var optionJson = new echartsOption()
            {
                title = new title()
                {
                    text = "近一天系统Cpu使用率"
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

        public ActionResult SystemCpuLineWeek()
        {
            Thread.Sleep(900);

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
                queryList.Add(Query<SystemCpu>.GTE(item => item.DateTimeNow, Convert.ToDateTime(DateTime.Now.AddDays(-7))));
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

            DateTime startDate = DateTime.Now.AddDays(-7);

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
                double pingLvCount = 0;
                int pingCount = 0;
                foreach (var item in List)
                {

                    if (item.DateTimeNow.Year == date.Year && item.DateTimeNow.Month == date.Month && item.DateTimeNow.Day == date.Day && (item.DateTimeNow.Hour + 8) == date.Hour && item.DateTimeNow.Minute == date.Minute)
                    {
                        //trueflag = true;

                        pingCount++;
                        pingLvCount += item.CpuUsageRate;

                    }
                }
                list.Add(new ValueText
                {
                    Text = date.ToString("yyyy年MM月dd日 HH时"),
                    Value = pingCount != 0 ? Math.Round((pingLvCount / pingCount), 1) : 0
                });
                //if (!trueflag)
                //{
                //    listTrue.Add(new ValueText
                //    {
                //        Text = date.ToString("yyyy-MM-dd HH:mm:ss"),
                //        Value = 0
                //    });
                //}
                date = date.AddHours(1);
            }

            var optionJson = new echartsOption()
            {
                title = new title()
                {
                    text = "近一周系统Cpu使用率"
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
        #endregion

        #region 系统硬盘统计
        public ActionResult SystemHdd()
        {
            #region 筛选条件
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
            #endregion

            //用于筛选路径
            var searchAddress = new Dictionary<string, string>();
            searchAddress.Add("Controller", RouteData.Values["controller"].ToString());
            searchAddress.Add("Action", RouteData.Values["action"].ToString() + "Echarts");
            ViewData["searchAddress"] = searchAddress;
            return PartialView();
        }


        public ActionResult SystemHddEcharts()
        {
            return PartialView();
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
                        text = "最新系统硬盘使用情况"
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

        //public ActionResult SystemHddLine()
        //{
        //    #region Request筛选
        //    string searchKeyWord = Request["SearchKeyWord"] ?? "";
        //    string searchDateTimeStart = Request["SearchDateTimeStart"] ?? "";
        //    string searchDateTimeEnd = Request["SearchDateTimeEnd"] ?? "";
        //    string SearchKu = Request["SearchKu"] ?? "";
        //    #endregion

        //    #region MongoDb筛选

        //    //mongodb筛选条件
        //    List<IMongoQuery> queryList = new List<IMongoQuery>();

        //    //获取最近三十天的数据
        //    queryList.Add(Query<SystemPing>.GTE(item => item.DateTimeNow, Convert.ToDateTime(DateTime.Now.AddDays(-30))));

        //    IMongoQuery query = null;

        //    if (!string.IsNullOrWhiteSpace(searchKeyWord))
        //    {
        //        queryList.Add(Query<SystemPing>.Matches(item => item.PingIP, searchKeyWord));
        //    }
        //    if (!string.IsNullOrWhiteSpace(searchDateTimeStart))
        //    {
        //        //queryList.Add(Query<SystemPing>.GTE(item => item.DateTimeNow, Convert.ToDateTime(searchDateTimeStart)));
        //    }
        //    if (!string.IsNullOrWhiteSpace(searchDateTimeEnd))
        //    {
        //        //queryList.Add(Query<SystemPing>.LTE(item => item.DateTimeNow, Convert.ToDateTime(searchDateTimeStart)));
        //    }
        //    if (queryList.Count > 0)
        //    {
        //        query = Query.And(queryList.ToArray());
        //    }
        //    #endregion

        //    #region SearchKu为空判断与替换

        //    string projectKu = string.Empty;
        //    if (!string.IsNullOrWhiteSpace(SearchKu))
        //    {
        //        projectKu = SearchKu.Replace('.', '-');
        //    }
        //    else
        //    {
        //        return Content("");
        //    }
        //    #endregion

        //    #region 数据库链接修改

        //    var mongoOther = MongoDBHelper.MongoConnModelList.FirstOrDefault(item => item.MongoConnType == MongoConnType.Other);
        //    if (mongoOther != null)
        //    {
        //        MongoDBHelper.MongoConnModelList.Remove(mongoOther);
        //    }
        //    MongoDBHelper.MongoConnModelList.Add(new MongoConnModel() { MongoConnType = MongoConnType.Other, MongoConnStr = "mongodb://" + MongoDBUserName + ":" + MongoDBUserPassWord + "@" + SearchKu + ":27017", MongoDatabase = "project" + projectKu });

        //    #endregion

        //    #region 查询
        //    var List = MongoDBHelper.Select<SystemCpu>(query, MongoConnType.Other);

        //    DateTime startMonth = DateTime.Now.AddDays(-30);

        //    var dataList = (from q in List
        //                    group q by new { Month = q.DateTimeNow.Month, Year = q.DateTimeNow.Year, Day = q.DateTimeNow.Day } into dsGroup
        //                    select new
        //                    {
        //                        Year = dsGroup.Key.Year,
        //                        Month = dsGroup.Key.Month,
        //                        Day = dsGroup.Key.Day,
        //                        Count = dsGroup.Count(),
        //                    }).OrderBy(u => u.Year);

        //    List<ValueText> list = new List<ValueText>();
        //    for (DateTime date = startMonth; date <= DateTime.Now; )
        //    {
        //        bool flag = false;
        //        foreach (var item in dataList)
        //        {
        //            if (item.Year == date.Year && item.Month == date.Month && item.Day == date.Day)
        //            {
        //                flag = true;
        //                list.Add(new ValueText
        //                {
        //                    Text = item.Year + "-" + item.Month + "-" + item.Day,
        //                    Value = item.Count
        //                });
        //            }
        //        }
        //        if (!flag)
        //        {
        //            list.Add(new ValueText
        //            {
        //                Text = date.Year + "-" + date.Month + "-" + date.Day,
        //                Value = 0
        //            });
        //        }
        //        date = date.AddDays(1);
        //    }

        //    var optionJson = new echartsOption()
        //    {
        //        title = new title()
        //        {
        //            text = "近三十天系统Cpu使用率"
        //        },
        //        tooltip = new tooltip()
        //        {
        //            trigger = "axis"
        //        },
        //        legend = new legend()
        //        {
        //            data = new string[] { "系统Cpu使用率" }
        //        },
        //        grid = new grid()
        //        {
        //            left = "3%",
        //            right = "4%",
        //            bottom = "3%",
        //            top = "8%",
        //            containLabel = true
        //        },
        //        toolbox = new toolbox() { feature = new feature() { } },
        //        xAxis = new xAxis()
        //        {
        //            type = "category",
        //            boundaryGap = false,
        //            data = list.Select(item => item.Text).ToArray()
        //        },
        //        yAxis = new yAxis()
        //        {
        //            type = "value"
        //        },
        //        series = new series[]
        //            {
        //                new series()
        //                { 
        //                    name="系统Cpu使用率", type="line",  stack="总量", data=list.Select(item=>item.Value).ToArray()
        //                }
        //            }
        //    };

        //    #endregion

        //    var stringJson = JsonConvert.SerializeObject(optionJson, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        //    return Content(stringJson);
        //}
        #endregion

        #region 系统内存统计
        public ActionResult SystemMemories()
        {
            #region 筛选条件
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
            #endregion

            //用于筛选路径
            var searchAddress = new Dictionary<string, string>();
            searchAddress.Add("Controller", RouteData.Values["controller"].ToString());
            searchAddress.Add("Action", RouteData.Values["action"].ToString() + "Echarts");
            ViewData["searchAddress"] = searchAddress;
            return PartialView();
        }

        public ActionResult SystemMemoriesEcharts()
        {
            return PartialView();
        }

        public ActionResult SystemMemoriesMinute()
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
                queryList.Add(Query<SystemMemories>.GTE(item => item.DateTimeNow, Convert.ToDateTime(DateTime.Now.AddMinutes(-1))));
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

            DateTime startDate = DateTime.Now.AddMinutes(-1);

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
                    text = "近一分钟系统内存使用率"
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
                    text = "近一小时系统内存使用率"
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

        public ActionResult SystemMemoriesDay()
        {
            Thread.Sleep(600);

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
                queryList.Add(Query<SystemMemories>.GTE(item => item.DateTimeNow, Convert.ToDateTime(DateTime.Now.AddDays(-1))));
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

            DateTime startDate = DateTime.Now.AddDays(-1);

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
                double pingLvCount = 0;
                int pingCount = 0;
                foreach (var item in List)
                {

                    if (item.DateTimeNow.Year == date.Year && item.DateTimeNow.Month == date.Month && item.DateTimeNow.Day == date.Day && (item.DateTimeNow.Hour + 8) == date.Hour && item.DateTimeNow.Minute == date.Minute)
                    {
                        //trueflag = true;

                        pingCount++;
                        pingLvCount += item.MemoryUsageRate;

                    }
                }
                list.Add(new ValueText
                {
                    Text = date.ToString("yyyy年MM月dd日 HH时mm分"),
                    Value = pingCount != 0 ? Math.Round((pingLvCount / pingCount), 1) : 0
                });
                //if (!trueflag)
                //{
                //    listTrue.Add(new ValueText
                //    {
                //        Text = date.ToString("yyyy-MM-dd HH:mm:ss"),
                //        Value = 0
                //    });
                //}
                date = date.AddMinutes(1);
            }

            var optionJson = new echartsOption()
            {
                title = new title()
                {
                    text = "近一天系统内存使用率"
                },
                tooltip = new tooltip()
                {
                    trigger = "axis"
                },
                legend = new legend()
                {
                    data = new string[] { "系统内存使用率(%)" }
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
                            name="系统内存使用率(%)", type="line",  stack="总量", data=list.Select(item=>item.Value).ToArray()
                        }
                    }
            };
            #endregion
            var stringJson = JsonConvert.SerializeObject(optionJson, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            return Content(stringJson);
        }

        public ActionResult SystemMemoriesWeek()
        {
            Thread.Sleep(900);

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
                queryList.Add(Query<SystemMemories>.GTE(item => item.DateTimeNow, Convert.ToDateTime(DateTime.Now.AddDays(-7))));
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

            DateTime startDate = DateTime.Now.AddDays(-7);

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
                double pingLvCount = 0;
                int pingCount = 0;
                foreach (var item in List)
                {

                    if (item.DateTimeNow.Year == date.Year && item.DateTimeNow.Month == date.Month && item.DateTimeNow.Day == date.Day && (item.DateTimeNow.Hour + 8) == date.Hour && item.DateTimeNow.Minute == date.Minute)
                    {
                        //trueflag = true;

                        pingCount++;
                        pingLvCount += item.MemoryUsageRate;

                    }
                }
                list.Add(new ValueText
                {
                    Text = date.ToString("yyyy年MM月dd日 HH时"),
                    Value = pingCount != 0 ? Math.Round((pingLvCount / pingCount), 1) : 0
                });
                //if (!trueflag)
                //{
                //    listTrue.Add(new ValueText
                //    {
                //        Text = date.ToString("yyyy-MM-dd HH:mm:ss"),
                //        Value = 0
                //    });
                //}
                date = date.AddHours(1);
            }

            var optionJson = new echartsOption()
            {
                title = new title()
                {
                    text = "近一周系统内存使用率"
                },
                tooltip = new tooltip()
                {
                    trigger = "axis"
                },
                legend = new legend()
                {
                    data = new string[] { "系统内存使用率(%)" }
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
                            name="系统内存使用率(%)", type="line",  stack="总量", data=list.Select(item=>item.Value).ToArray()
                        }
                    }
            };
            #endregion
            var stringJson = JsonConvert.SerializeObject(optionJson, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            return Content(stringJson);
        }
        #endregion

        #region 系统网络统计
        public ActionResult SystemInternet()
        {
            #region 筛选条件
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
            #endregion

            //用于筛选路径
            var searchAddress = new Dictionary<string, string>();
            searchAddress.Add("Controller", RouteData.Values["controller"].ToString());
            searchAddress.Add("Action", RouteData.Values["action"].ToString() + "Echarts");
            ViewData["searchAddress"] = searchAddress;
            return PartialView();
        }

        public ActionResult SystemInternetEcharts()
        {
            return PartialView();
        }

        public ActionResult SystemInternetMinute()
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
                queryList.Add(Query<SystemMemories>.GTE(item => item.DateTimeNow, Convert.ToDateTime(DateTime.Now.AddMinutes(-1))));
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

            DateTime startDate = DateTime.Now.AddMinutes(-1);

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
                    text = "近一分钟系统网络使用情况"
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
                    text = "近一小时系统网络使用情况"
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

        public ActionResult SystemInternetDay()
        {
            Thread.Sleep(600);

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
                queryList.Add(Query<SystemMemories>.GTE(item => item.DateTimeNow, Convert.ToDateTime(DateTime.Now.AddDays(-1))));
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

            DateTime startDate = DateTime.Now.AddDays(-1);

            List<ValueText> list = new List<ValueText>();
            for (DateTime date = startDate; date <= DateTime.Now; )
            {
                double upCount = 0;
                int up = 0;
                double downCount = 0;
                int down = 0;
                foreach (var item in List)
                {

                    if (item.DateTimeNow.Year == date.Year && item.DateTimeNow.Month == date.Month && item.DateTimeNow.Day == date.Day && (item.DateTimeNow.Hour + 8) == date.Hour && item.DateTimeNow.Minute == date.Minute)
                    {
                        up++;
                        upCount += double.Parse(item.UploadByte);
                        down++;
                        downCount += double.Parse(item.DownloadByte);

                    }
                }
                list.Add(new ValueText
                {
                    Text = date.ToString("yyyy年MM月dd日 HH时mm分"),
                    Value = up != 0 ? Math.Round((upCount / up), 1) : 0,
                    ViceValue = down != 0 ? Math.Round((downCount / down), 1) : 0
                });
                date = date.AddMinutes(1);
            }

            var optionJson = new echartsOption()
            {
                title = new title()
                {
                    text = "近一天系统网络使用情况"
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

        public ActionResult SystemInternetWeek()
        {
            Thread.Sleep(900);

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
                queryList.Add(Query<SystemMemories>.GTE(item => item.DateTimeNow, Convert.ToDateTime(DateTime.Now.AddDays(-7))));
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

            DateTime startDate = DateTime.Now.AddDays(-7);

            List<ValueText> list = new List<ValueText>();
            for (DateTime date = startDate; date <= DateTime.Now; )
            {
                double upCount = 0;
                int up = 0;
                double downCount = 0;
                int down = 0;
                foreach (var item in List)
                {

                    if (item.DateTimeNow.Year == date.Year && item.DateTimeNow.Month == date.Month && item.DateTimeNow.Day == date.Day && (item.DateTimeNow.Hour + 8) == date.Hour)
                    {
                        up++;
                        upCount += double.Parse(item.UploadByte);
                        down++;
                        downCount += double.Parse(item.DownloadByte);

                    }
                }
                list.Add(new ValueText
                {
                    Text = date.ToString("yyyy年MM月dd日 HH时"),
                    Value = up != 0 ? Math.Round((upCount / up), 1) : 0,
                    ViceValue = down != 0 ? Math.Round((downCount / down), 1) : 0
                });
                date = date.AddHours(1);
            }

            var optionJson = new echartsOption()
            {
                title = new title()
                {
                    text = "近一周系统网络使用情况"
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

        #region 进程Cpu统计
        public ActionResult ProcessCpu()
        {
            #region 筛选条件
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
            #endregion

            //用于筛选路径
            var searchAddress = new Dictionary<string, string>();
            searchAddress.Add("Controller", RouteData.Values["controller"].ToString());
            searchAddress.Add("Action", RouteData.Values["action"].ToString() + "Echarts");
            ViewData["searchAddress"] = searchAddress;
            return PartialView();
        }

        public ActionResult ProcessCpuEcharts()
        {
            return PartialView();
        }

        public ActionResult ProcessCpuMinute()
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
                queryList.Add(Query<ProcessCpu>.GTE(item => item.DateTimeNow, Convert.ToDateTime(DateTime.Now.AddMinutes(-1))));
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

            DateTime startDate = DateTime.Now.AddMinutes(-1);

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
                    text = "近一分钟进程Cpu使用情况"
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
                    text = "近一小时进程Cpu使用情况"
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

        public ActionResult ProcessCpuDay()
        {
            Thread.Sleep(600);

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
                queryList.Add(Query<ProcessCpu>.GTE(item => item.DateTimeNow, Convert.ToDateTime(DateTime.Now.AddDays(-1))));
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

            DateTime startDate = DateTime.Now.AddDays(-1);

            List<ValueText> list = new List<ValueText>();
            foreach (var hddName in hddNames)
            {
                var valueList = List.Where(l => l.ProcessName == hddName.Name);
                for (DateTime date = startDate; date <= DateTime.Now; )
                {
                    int count = 0;
                    double valueSum = 0;
                    foreach (var item in valueList)
                    {
                        if (item.DateTimeNow.Year == date.Year && item.DateTimeNow.Month == date.Month && item.DateTimeNow.Day == date.Day && (item.DateTimeNow.Hour + 8) == date.Hour && item.DateTimeNow.Minute == date.Minute)
                        {
                            count++;
                            valueSum += (double)item.ProcessUsageRate;
                        }
                    }
                    list.Add(new ValueText
                    {
                        Text = date.ToString("yyyy年MM月dd日 HH时mm分"),
                        Value = count != 0 ? Math.Round((valueSum / count), 3) : 0
                    });
                    date = date.AddMinutes(1);
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
                    text = "近一天进程Cpu使用情况"
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

        public ActionResult ProcessCpuWeek()
        {
            Thread.Sleep(900);

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
                queryList.Add(Query<ProcessCpu>.GTE(item => item.DateTimeNow, Convert.ToDateTime(DateTime.Now.AddDays(-7))));
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

            DateTime startDate = DateTime.Now.AddDays(-7);

            List<ValueText> list = new List<ValueText>();
            foreach (var hddName in hddNames)
            {
                var valueList = List.Where(l => l.ProcessName == hddName.Name);
                for (DateTime date = startDate; date <= DateTime.Now; )
                {
                    int count = 0;
                    double valueSum = 0;
                    foreach (var item in valueList)
                    {
                        if (item.DateTimeNow.Year == date.Year && item.DateTimeNow.Month == date.Month && item.DateTimeNow.Day == date.Day && (item.DateTimeNow.Hour + 8) == date.Hour)
                        {
                            count++;
                            valueSum += (double)item.ProcessUsageRate;
                        }
                    }
                    list.Add(new ValueText
                    {
                        Text = date.ToString("yyyy年MM月dd日 HH时"),
                        Value = count != 0 ? Math.Round((valueSum / count), 3) : 0
                    });
                    date = date.AddHours(1);
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
                    text = "近一周进程Cpu使用情况"
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
        #endregion

        #region 进程内存统计
        public ActionResult ProcessMemories()
        {
            #region 筛选条件
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
            #endregion

            //用于筛选路径
            var searchAddress = new Dictionary<string, string>();
            searchAddress.Add("Controller", RouteData.Values["controller"].ToString());
            searchAddress.Add("Action", RouteData.Values["action"].ToString() + "Echarts");
            ViewData["searchAddress"] = searchAddress;
            return PartialView();
        }

        public ActionResult ProcessMemoriesEcharts()
        {
            return PartialView();
        }

        public ActionResult ProcessMemoriesMinute()
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
                queryList.Add(Query<SystemMemories>.GTE(item => item.DateTimeNow, Convert.ToDateTime(DateTime.Now.AddMinutes(-1))));
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

            DateTime startDate = DateTime.Now.AddMinutes(-1);

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
                    text = "近一分钟进程内存使用情况"
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
                    text = "近一小时进程内存使用情况"
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

        public ActionResult ProcessMemoriesDay()
        {
            Thread.Sleep(600);

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
                queryList.Add(Query<SystemMemories>.GTE(item => item.DateTimeNow, Convert.ToDateTime(DateTime.Now.AddDays(-1))));
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

            DateTime startDate = DateTime.Now.AddDays(-1);

            List<ValueText> list = new List<ValueText>();
            foreach (var hddName in hddNames)
            {
                var valueList = List.Where(l => l.ProcessName == hddName.Name);
                for (DateTime date = startDate; date <= DateTime.Now; )
                {
                    int count = 0;
                    double valueSum = 0;
                    foreach (var item in valueList)
                    {
                        if (item.DateTimeNow.Year == date.Year && item.DateTimeNow.Month == date.Month && item.DateTimeNow.Day == date.Day && (item.DateTimeNow.Hour + 8) == date.Hour && item.DateTimeNow.Minute == date.Minute)
                        {
                            count++;
                            valueSum += (double)item.ProcessSpecialUseKB;
                        }
                    }
                    list.Add(new ValueText
                    {
                        Text = date.ToString("yyyy年MM月dd日 HH时mm分"),
                        Value = count != 0 ? Math.Round((valueSum / count), 3) : 0
                    });
                    date = date.AddMinutes(1);
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
                    text = "近一天进程内存使用情况"
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

        public ActionResult ProcessMemoriesWeek()
        {
            Thread.Sleep(900);

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
                queryList.Add(Query<SystemMemories>.GTE(item => item.DateTimeNow, Convert.ToDateTime(DateTime.Now.AddDays(-7))));
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

            DateTime startDate = DateTime.Now.AddDays(-7);

            List<ValueText> list = new List<ValueText>();
            foreach (var hddName in hddNames)
            {
                var valueList = List.Where(l => l.ProcessName == hddName.Name);
                for (DateTime date = startDate; date <= DateTime.Now; )
                {
                    int count = 0;
                    double valueSum = 0;
                    foreach (var item in valueList)
                    {
                        if (item.DateTimeNow.Year == date.Year && item.DateTimeNow.Month == date.Month && item.DateTimeNow.Day == date.Day && (item.DateTimeNow.Hour + 8) == date.Hour)
                        {
                            count++;
                            valueSum += (double)item.ProcessSpecialUseKB;
                        }
                    }
                    list.Add(new ValueText
                    {
                        Text = date.ToString("yyyy年MM月dd日 HH时"),
                        Value = count != 0 ? Math.Round((valueSum / count), 3) : 0
                    });
                    date = date.AddHours(1);
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
                    text = "近一周进程内存使用情况"
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
        #endregion
    }
}
