#region << 版 本 注 释 >>
/*
     * ========================================================================
     * Copyright Notice © 2010-2014 TideBuy.com All rights reserved .
     * ========================================================================
     * 机器名称：USER-429236GLDJ 
     * 文件名：  WindowsServiceManangerHelper 
     * 版本号：  V1.0.0.0 
     * 创建人：  杨焕文 
     * 创建时间：2014/10/24 11:34:24 
     * 描述    : 服务相关
     * =====================================================================
     * 修改时间：2014/10/24 11:34:24 
     * 修改人  ：  
     * 版本号  ： V1.0.0.0 
     * 描述    ：
*/
#endregion
using System;
using System.Collections;
using System.Configuration.Install;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using Microsoft.Win32;

namespace TLZ.Helper
{
   public static class WindowsServiceManangerHelper
    {

        /// <summary>
        /// 获取服务安装路径
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public static string GetWindowsServiceInstallPath(string serviceName)
        {
            string directory = null;
            string key = @"SYSTEM\CurrentControlSet\Services\" + serviceName;
            try
            {
                using (RegistryKey openSubKey = Registry.LocalMachine.OpenSubKey(key))
                {
                    if (openSubKey != null)
                    {
                        string path = openSubKey.GetValue("ImagePath").ToString();
                        path = path.Replace("\"", string.Empty);   //替换掉双引号  
                        FileInfo fi = new FileInfo(path);
                        directory = fi.Directory + "\\";
                    }
                }
            }
            catch (Exception ex)
            {
                directory = null;
                throw ex;
            }
            return directory;
        }

        /// <summary>
        /// 根据服务名称获取服务状态。
        /// </summary>
        /// <param name="serviceName">服务名</param>
        /// <returns>状态</returns>
        public static int GetServiceStatus(string serviceName)
        {
            int result = 0;
            try
            {
                using (ServiceController sc = new ServiceController(serviceName))
                {
                    result = TypeParseHelper.StrToInt32(sc.Status);
                }
            }
            catch (Exception ex)
            {
                result = 0;
                throw ex;
            }
            return result;
        }



        /// <summary>
        /// 检查服务存在的存在性
        /// </summary>
        /// <param name="serviceName">服务名</param>
        /// <returns>存在返回 true,否则返回 false;</returns>
        public static bool IsServiceIsExisted(string serviceName)
        {
            ServiceController[] services = ServiceController.GetServices();
            return services.Any(s => s.ServiceName.ToLower() == serviceName.ToLower());
        }
        /// <summary>
        /// 安装Windows服务
        /// </summary>
        /// <param name="stateSaver">集合</param>
        /// <param name="filepath">程序文件路径</param>
        public static void InstallmyService(IDictionary stateSaver, string filepath)
        {
            try
            {
                using (AssemblyInstaller assemblyInstaller = new AssemblyInstaller
                {
                    UseNewContext = true,
                    Path = filepath
                })
                {
                    assemblyInstaller.Install(stateSaver);
                    assemblyInstaller.Commit(stateSaver);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 卸载Windows服务
        /// </summary>
        /// <param name="filepath">程序文件路径</param>
        public static void UnInstallmyService(string filepath)
        {
            try
            {
                using (AssemblyInstaller assemblyInstaller = new AssemblyInstaller
                {
                    UseNewContext = true,
                    Path = filepath
                })
                {
                    assemblyInstaller.Uninstall(null);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 启动服务
        /// </summary>
        /// <param name="serviceName">服务名</param>
        /// <returns>存在返回 true,否则返回 false;</returns>
        public static bool RunService(string serviceName)
        {
            bool result = true;
            try
            {
                using (ServiceController sc = new ServiceController(serviceName))
                {
                    if (sc.Status.Equals(ServiceControllerStatus.Stopped)
                        || sc.Status.Equals(ServiceControllerStatus.StopPending))
                    {
                        sc.Start();
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                throw ex;
            }

            return result;
        }
        /// <summary>
        /// 停止服务
        /// </summary>
        /// <param name="serviceName">服务名</param>
        /// <returns>存在返回 true,否则返回 false;</returns>
        public static bool StopService(string serviceName)
        {
            bool result = true;
            try
            {
                using (ServiceController sc = new ServiceController(serviceName))
                {
                    if (!sc.Status.Equals(ServiceControllerStatus.Stopped))
                    {
                        sc.Stop();
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                throw ex;
            }
            return result;
        }
        /// <summary>
        /// 获取指定服务的版本号
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <returns></returns>
        public static string GetServiceVersion(string serviceName)
        {
            string result = null;
            if (string.IsNullOrEmpty(serviceName))
            {
                return result;
            }
            try
            {
                string path = GetWindowsServiceInstallPath(serviceName) + "\\" + serviceName + ".exe";
                Assembly assembly = Assembly.LoadFile(path);
                AssemblyName assemblyName = assembly.GetName();
                Version version = assemblyName.Version;
                result = version.ToString();
            }
            catch (Exception ex)
            {
                result = null;
                throw ex;
            }
            return result;
        }

        /// <summary>
        /// 重启windows服务
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public static bool ResetService(string serviceName)
        {
            bool result = true;
            try
            {
                using (ServiceController service = new ServiceController(serviceName))
                {
                    if (service.Status == ServiceControllerStatus.Running)
                    {
                        service.Stop();
                        service.WaitForStatus(ServiceControllerStatus.Stopped);
                    }
                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running);
                }
            }
            catch (Exception ex)
            {
                result = false;
                throw ex;
            }
            return result;
        }

   

    }
}
