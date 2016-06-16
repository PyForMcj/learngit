#region << 版 本 注 释 >>
/*
     * ========================================================================
     * Copyright Notice © 2010-2014 TideBuy.com All rights reserved .
     * ========================================================================
     * 机器名称：YANGHUANWEN 
     * 文件名：  ConfigHelper 
     * 版本号：  V1.0.0.0 
     * 创建人：  Administrator 
     * 创建时间：2014/9/22 11:29:22 
     * 描述    :
     * =====================================================================
     * 修改时间：2014/9/22 11:29:22 
     * 修改人  ：  
     * 版本号  ： V1.0.0.0 
     * 描述    ：
*/
#endregion
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Configuration;

namespace TLZ.Helper
{
    public class ConfigHelper
    {
        /// <summary>
        /// 读取Config文件里面的AppSetting的Value
        /// </summary>
        /// <param name="exeConfigPath">读取Config文件的名称（包括路径在内） 注意：是.exe文件的路径,不是.exe.config</param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetAppSettingValue(string exeConfigPath, string key)
        {
            Configuration config = null;
            try
            {
                config = ConfigurationManager.OpenExeConfiguration(exeConfigPath);
            }
            catch (Exception)
            {
                config = null;
            }
            if (config != null && config.HasFile)
            {
                foreach (KeyValueConfigurationElement item in config.AppSettings.Settings)
                {
                    if (item.Key.Equals(key, StringComparison.CurrentCultureIgnoreCase))
                    {
                        return item.Value;
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// 保存Config文件里面的AppSetting的Value
        /// </summary>
        /// <param name="exeConfigPath">要保存的Config文件的名称（包括路径在内）</param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SaveAppSettingValue(string exeConfigPath, string key, string value)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(exeConfigPath);
            if (config.HasFile)
            {
                config.AppSettings.Settings[key].Value = value;
                config.Save(ConfigurationSaveMode.Minimal);
                ConfigurationManager.RefreshSection("appSettings");
            }
        }
        /// <summary>
        /// 保存Config文件里面的AppSetting的Value
        /// </summary>
        /// <param name="exeConfigPath">要保存的Config文件的名称（包括路径在内）</param>
        /// <param name="dict"></param>
        public static void SaveAppSettingValue(string exeConfigPath, Dictionary<string, string> dict)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(exeConfigPath);
            foreach (KeyValuePair<string, string> pair in dict)
            {
                config.AppSettings.Settings[pair.Key].Value = pair.Value;
            }
            config.Save(ConfigurationSaveMode.Minimal);
        }
        /// <summary>
        /// 读取appSettings中的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetAppSettingValue(string key)
        {
            string value = null;
            foreach (string item in ConfigurationManager.AppSettings)
            {
                if (item.Equals(key, StringComparison.CurrentCultureIgnoreCase))
                {
                    value = ConfigurationManager.AppSettings[key];
                    break;
                }
            }
            return value;
        }

        /// <summary>
        /// 读取appSettings中的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="default">默认值</param>
        /// <returns></returns>
        public static string GetAppSettingValueWithDefault(string key, string @default)
        {
            string value = GetAppSettingValue(key);
            if (string.IsNullOrWhiteSpace(value))
            {
                value = @default;
            }
            return value;
        }

        public static void SaveAppSettingValue(string key, string value)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings[key].Value = value;
            config.Save(ConfigurationSaveMode.Minimal);
            ConfigurationManager.RefreshSection("appSettings");
        }

        /// <summary>
        /// 修改网站web.config的AppSetting中的值
        /// Date:2015年1月22日
        /// Author:陈锐
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SaveWebConfigSettingValue(string virtualPath, string key, string value)
        {
            Configuration config = WebConfigurationManager.OpenWebConfiguration(virtualPath);
            AppSettingsSection appSetting = config.AppSettings;
            appSetting.Settings[key].Value = value;
            config.Save(ConfigurationSaveMode.Modified);
        }

        /// <summary>
        /// 读取connectionStrings中的连接字符串
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetConnectionString(string name)
        {
            string value = null;
            foreach (ConnectionStringSettings connectionStringSettings in ConfigurationManager.ConnectionStrings)
            {
                if (connectionStringSettings.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                {
                    value = connectionStringSettings.ConnectionString;
                    break;
                }
            }
            return value;
        }
        public static void SaveConnectionString(string name, string value)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.ConnectionStrings.ConnectionStrings[name].ConnectionString = value;
            config.Save(ConfigurationSaveMode.Minimal);
            ConfigurationManager.RefreshSection("connectionStrings");
        }
    }
}
