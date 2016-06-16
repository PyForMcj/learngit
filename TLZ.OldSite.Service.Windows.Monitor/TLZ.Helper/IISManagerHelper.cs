#region << 版 本 注 释 >>
/*
     * ========================================================================
     * Copyright Notice © 2010-2014 TideBuy.com All rights reserved .
     * ========================================================================
     * 机器名称：YANGHUANWEN 
     * 文件名：  IISManager 
     * 版本号：  V1.0.0.0 
     * 创建人：  杨焕文 
     * 创建时间：2014/10/27 10:41:33 
     * 描述    :IIS应用程序池辅助类
     * =====================================================================
     * 修改时间：2014/10/27 10:41:33 
     * 修改人  ：  
     * 版本号  ： V1.0.0.0 
     * 描述    ：
*/
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using Microsoft.Web.Administration;

namespace TLZ.Helper
{

    /// <summary>
    /// IIS应用程序池辅助类
    /// </summary>
    public class IISManagerHelper
    {
        protected static string Host = "localhost";

        private static Hashtable _hs; //创建哈希表,保存池中的站点

        private static List<string> _pls; //池数组

        /// <summary>
        ///  取得所有应用程序池
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAppPools()
        {
            var appPools = new DirectoryEntry(string.Format("IIS://{0}/W3SVC/AppPools", Host));
            return (from DirectoryEntry entry in appPools.Children select entry.Name).ToList();
        }

        public static PropertyCollection GetAppPoolProperties(string appPoolName)
        {
            PropertyCollection propertyCollection = null;
            var appPools = new DirectoryEntry(string.Format("IIS://{0}/W3SVC/AppPools", Host));
            foreach (DirectoryEntry entry in appPools.Children)
            {
                if (entry.Name == appPoolName)
                {
                    propertyCollection = entry.Properties;
                }
            }
            return propertyCollection;
        }

        /// <summary>
        /// 获取应用程序数量
        /// </summary>
        /// <param name="appName"></param>
        /// <returns></returns>
        public static int GetAppNumber(string appName)
        {
            int appNumber;
            _hs = new Hashtable();
            _pls = GetAppPools(); //获取应用程序池名称数组    
            foreach (string i in _pls) //填充哈希表key值内容
            {
                _hs.Add(i, "");
            }
            GetPoolWeb();
            Dictionary<string, int> dic = new Dictionary<string, int>();
            foreach (string i in _pls)
            {
                appNumber = _hs[i].ToString() != "" ? Convert.ToInt32(_hs[i].ToString().Split(',').Length.ToString()) : 0;
                dic.Add(i, appNumber);
            }
            dic.TryGetValue(appName, out appNumber);
            return appNumber;

        }

        /// <summary>
        /// 获取IIS版本
        /// </summary>
        /// <returns></returns>
        public static string GetIisVersion()
        {
            string version = string.Empty;
            try
            {
                DirectoryEntry getEntity = new DirectoryEntry("IIS://LOCALHOST/W3SVC/INFO");
                version = getEntity.Properties["MajorIISVersionNumber"].Value.ToString();
            }
            catch (Exception se)
            {
                //说明一点:IIS5.0中没有(int)entry.Properties["MajorIISVersionNumber"].Value;属性，将抛出异常 证明版本为 5.0

            }
            return version;
        }

        /// <summary>
        /// 判断程序池是否存在
        /// </summary>
        /// <param name="appPoolName">程序池名称</param>
        /// <returns>true存在 false不存在</returns>
        public static bool IsAppPoolExsit(string appPoolName)
        {
            bool result = false;
            var appPools = new DirectoryEntry(string.Format("IIS://{0}/W3SVC/AppPools", Host));
            foreach (DirectoryEntry entry in appPools.Children)
            {
                if (!entry.Name.Equals(appPoolName)) continue;
                result = true;
                break;
            }
            return result;
        }

        /// <summary>
        /// 创建应用程序池
        /// </summary>
        /// <param name="appPool"></param>
        /// <returns></returns>
        public static bool CreateAppPool(string appPool)
        {
            try
            {
                if (!IsAppPoolExsit(appPool))
                {
                    var appPools = new DirectoryEntry(string.Format("IIS://{0}/W3SVC/AppPools", Host));
                    DirectoryEntry entry = appPools.Children.Add(appPool, "IIsApplicationPool");
                    entry.CommitChanges();
                    return true;
                }
            }
            catch
            {
                return false;
            }
            return false;
        }

        /// <summary>
        /// 编辑应用程序池
        /// </summary>
        /// <param name="application"></param>
        /// <returns></returns>
        public static bool EditAppPool(ApplicationPool application)
        {
            try
            {
                if (IsAppPoolExsit(application.Name))
                {
                    var manager = new ServerManager();
                    manager.ApplicationPools[application.Name].ManagedRuntimeVersion = application.ManagedRuntimeVersion;
                    manager.ApplicationPools[application.Name].ManagedPipelineMode = application.ManagedPipelineMode;
                    //托管模式Integrated为集成 Classic为经典
                    manager.CommitChanges();
                    return true;
                }
            }
            catch
            {
                return false;
            }
            return false;
        }

        /// <summary>
        /// 操作应用程序池
        /// </summary>
        /// <param name="appPoolName"></param>
        /// <param name="method">Start==启动 Recycle==回收 Stop==停止</param>
        /// <returns></returns>
        public static bool DoAppPool(string appPoolName, string method)
        {
            bool result = false;
            try
            {
                var appPools = new DirectoryEntry(string.Format("IIS://{0}/W3SVC/AppPools", Host));
                DirectoryEntry findPool = appPools.Children.Find(appPoolName, "IIsApplicationPool");
                findPool.Invoke(method, null);
                appPools.CommitChanges();
                appPools.Close();
                result = true;
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 删除指定程序池
        /// </summary>
        /// <param name="appPoolName">程序池名称</param>
        /// <returns>true删除成功 false删除失败</returns>
        public static bool DeleteAppPool(string appPoolName)
        {
            bool result = false;
            var appPools = new DirectoryEntry(string.Format("IIS://{0}/W3SVC/AppPools", Host));
            foreach (DirectoryEntry entry in appPools.Children)
            {
                if (!entry.Name.Equals(appPoolName)) continue;
                try
                {
                    entry.DeleteTree();
                    result = true;
                    break;
                }
                catch
                {
                    result = false;
                }
            }
            return result;
        }

        /// <summary>
        /// 获得所有的应用程序池和对应站点
        /// </summary>
        private static void GetPoolWeb()
        {
            DirectoryEntry root = new DirectoryEntry("IIS://localhost/W3SVC");
            foreach (DirectoryEntry website in root.Children)
            {
                if (website.SchemaClassName != "IIsWebServer") continue;
                string comment = website.Properties["ServerComment"][0].ToString();
                DirectoryEntry siteVDir = website.Children.Find("Root", "IISWebVirtualDir");
                string poolname = siteVDir.Properties["AppPoolId"][0].ToString().Trim();
                if (string.IsNullOrWhiteSpace(poolname))
                {
                    poolname = website.Properties["AppPoolId"][0].ToString().Trim();
                }
                foreach (string i in _pls)
                {
                    if (i != poolname) continue;
                    if (_hs[i].ToString() == "")
                        _hs[i] = comment;
                    else _hs[i] += "," + comment;
                }
            }
            root.Close();
        }
    }


}

