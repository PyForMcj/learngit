#region << 版 本 注 释 >>
/*
     * ========================================================================
     * Copyright Notice © 2010-2014 TideBuy.com All rights reserved .
     * ========================================================================
     * 机器名称： 
     * 文件名：  FileLogger 
     * 版本号：  V1.0.0.0 
     * 创建人：  王云鹏 
     * 创建时间：2014/7/11
     * 描述    : 将日志写入到Widnows事件查看器
     * =====================================================================
     * 修改时间：2014/7/11
     * 修改人  ：  
     * 版本号  ：V1.0.0.0 
     * 描述    ：将日志写入到Widnows事件查看器
*/
#endregion
using System;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace TLZ.Logger
{
    /// <summary>
    /// FileLogger 将日志记录到Log4net配置里。FileLogger是线程安全的。
    /// </summary>
    public class FileLogger : ILogger
    {
        private string _configLogLevel = null;
        private log4net.ILog _log = null;
        private bool _enabled = true;

        #region 构造函数
        public FileLogger()
        {
            _configLogLevel = GetAppSettingValue("LogLevel");
            string logName = GetAppSettingValue("LogName");
            string configPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.xml");
            log4net.Config.XmlConfigurator.Configure(new FileInfo(configPath));
            _log = log4net.LogManager.GetLogger(logName);
        }
        #endregion
        /// <summary>
        /// 读取appSettings中的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private static string GetAppSettingValue(string key)
        {
            string value = null;
            foreach (string item in System.Configuration.ConfigurationManager.AppSettings)
            {
                if (item.Equals(key, StringComparison.CurrentCultureIgnoreCase))
                {
                    value = System.Configuration.ConfigurationManager.AppSettings[key];
                    break;
                }
            }
            return value;
        }
        #region ILogger 成员

        #region Log
        void Log(string msg, ELogLevel logLevel)
        {
            if (string.IsNullOrWhiteSpace(msg) || !_enabled)
                return;
#if DEBUG
            Trace.TraceInformation(msg);
#endif
            if (string.IsNullOrWhiteSpace(_configLogLevel))
                _configLogLevel = ((int)ELogLevel.Error).ToString();

            int configLogLevel = Convert.ToInt32(_configLogLevel);
            if ((int)logLevel <= configLogLevel)
            {//传过来的日志级别 <= 配置文件的日志级别都会记录到日志文件里
                try
                {
                    switch (logLevel)
                    {
                        case ELogLevel.Error:
                            _log.Error(msg);
                            break;
                        case ELogLevel.Trace:
                            _log.Warn(msg);
                            break;
                        case ELogLevel.Debug:
                            _log.Debug(msg);
                            break;
                        case ELogLevel.Info:
                            _log.Info(msg);
                            break;
                    }
                }
                catch
                { }
            }
        }
        #endregion

        #region LogWithTime
        public void LogWithTime(string msg, ELogLevel logLevel = ELogLevel.Info)
        {
            string formatMsg = null;
            switch (logLevel)
            {
                case ELogLevel.Trace:
                    formatMsg = string.Format("\t{0}", msg);
                    break;
                case ELogLevel.Info:
                    formatMsg = string.Format("\t{0}", msg);
                    break;
                case ELogLevel.Debug:
                    formatMsg = string.Format("\t{0}", msg);
                    break;
                case ELogLevel.Error:
                    formatMsg = string.Format("\t{0}", msg);
                    break;
                default:
                    return;
            }

            Log(formatMsg, logLevel);
        }
        #endregion

        #region Enabled
        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }
        #endregion

        #endregion

        #region IDisposable 成员

        public void Dispose()
        {
        }

        #endregion
    }
}
