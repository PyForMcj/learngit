using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using TLZ.Helper;
using TLZ.MongoDB;
using TLZ.OldSite.DB.MongoDB.Model;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Echevil;

namespace TLZ.OldSite.Service.Windows.Monitor.Host.ConsoleApplication
{
    class Program
    {
        /// <summary>
        /// 系统进程帮助类
        /// </summary>
        static ProcessHelper prohelper;
        static string systemAddress;
        static LogWritesHelper logHelper;
        static Program()
        {
            logHelper = new LogWritesHelper();
            prohelper = new ProcessHelper();

            if (ConfigurationManager.AppSettings["SystemAddress"] != null)
            {
                systemAddress = ConfigurationManager.AppSettings["SystemAddress"];
            }
            else
                throw new Exception("未配置对应的系统区域AppSetting");
        }
        static void Main(string[] args)
        {
            //NetworkMonitor monitor = new NetworkMonitor();
            //NetworkAdapter[] adapters = monitor.Adapters;

            //// adapters 的长度是0，机器上没安装任何网络设备
            //if (adapters.Length == 0)
            //{
            //    Console.WriteLine("No network adapters found on this computer.");
            //    return;
            //}

            //// 开启一个定时器用以每秒钟计算数据包
            //monitor.StartMonitoring();

            //while (true)
            //{
            //    // Name属性指示网络适配器的名字
            //    Console.WriteLine(adapters[0].Name);
            //    // 下面的参数用以衡量网络上下传的速度：DownloadSpeedKbps，UploadSpeedKbps，UploadSpeed和DownloadSpeed。后面两个是整数值表示每秒的字节数。
            //    Console.WriteLine(String.Format("下载：{0:n}Kbps \t\t 上传：{1:n} Kbps",
            //        adapters[0].DownloadSpeedKbps,
            //       adapters[0].UploadSpeedKbps));

            //    //foreach (NetworkAdapter adapter in adapters)
            //    //{
            //    //    // Name属性指示网络适配器的名字
            //    //    Console.WriteLine(adapter.Name);
            //    //    // 下面的参数用以衡量网络上下传的速度：DownloadSpeedKbps，UploadSpeedKbps，UploadSpeed和DownloadSpeed。后面两个是整数值表示每秒的字节数。
            //    //    Console.WriteLine(String.Format("下载：{0:n}Kbps \t\t 上传：{1:n} Kbps",
            //    //        adapter.DownloadSpeedKbps,
            //    //       adapter.UploadSpeedKbps));
            //    //}
            //    System.Threading.Thread.Sleep(10000); // Sleeps for one second.
            //}

            //// 停止定时器，adapter 的属性也不再可用
            //monitor.StopMonitoring();


            #region 插入数据
            for (int i = 0; i < 1; i++)
            {
                MongoDBHelper.Insert<Users>(
                               new Users()
                               {
                                   UserName = "admin",
                                   PassWord="admin",
                                   PersonID="5760ba7af0308a1ebcb68779",
                                   DateTimeNow = DateTime.Now
                               }, MongoConnType.Center);
            }            
            Console.ReadKey();
            ////ThreadMonitorService test = new ThreadMonitorService();
            //#endregion
            //while (true)
            //{


            //string pingrst = string.Empty;

            //Process p = new Process();
            //p.StartInfo.FileName = "cmd.exe";
            //p.StartInfo.UseShellExecute = false;
            //p.StartInfo.RedirectStandardInput = true;
            //p.StartInfo.RedirectStandardOutput = true;
            //p.StartInfo.RedirectStandardError = true;
            //p.StartInfo.CreateNoWindow = true;
            //p.Start();
            //p.StandardInput.WriteLine("ping -n 1 103.235.223.120");
            //p.StandardInput.WriteLine("exit");
            //string strRst = p.StandardOutput.ReadToEnd();

            //if (strRst.IndexOf("Ping 统计信息:") != -1)
            //{
            //    pingrst = "请求成功";
            //}
            //else if (strRst.IndexOf("请求超时。") != -1)
            //    pingrst = "请求超时";
            //else if (strRst.IndexOf("请求找不到主机") != -1)
            //    pingrst = "请求找不到主机";
            //else if (strRst.IndexOf("不知名主机") != -1)
            //    pingrst = "请求无法解析主机";
            //else
            //    pingrst = strRst;
            //p.Close();

            //var model = new SystemPing() { };

            //strRst = Regex.Replace(strRst, @"\s", "");

            //string ziJie = strRst.Substring(strRst.IndexOf("字节=")+3);
            //model.PingByte = double.Parse(ziJie.Substring(0, ziJie.IndexOf("时间=")));
            //string shiJian = strRst.Substring(strRst.IndexOf("时间=") + 3);
            //model.PingDate = double.Parse(shiJian.Substring(0, shiJian.IndexOf("TTL=") - 2));
            //string ttl = strRst.Substring(strRst.IndexOf("TTL=") + 4);
            //model.PingTTL = double.Parse(ttl.Substring(0, ttl.IndexOf("103.235.223.120的Ping统计信息:")));
            //string yiFaSong = strRst.Substring(strRst.IndexOf("已发送=") + 4);
            //model.PingFaSong = double.Parse(yiFaSong.Substring(0, yiFaSong.IndexOf("已接收=")-1));
            //string yiJieShou = strRst.Substring(strRst.IndexOf("已接收=") + 4);
            //model.PingJieShou = double.Parse(yiJieShou.Substring(0, yiJieShou.IndexOf("丢失=") - 1));
            //string diuShi = strRst.Substring(strRst.IndexOf("丢失=") + 3);
            //model.PingDiuShi = double.Parse(diuShi.Substring(0, diuShi.IndexOf("(")));
            //string diuShiLv = strRst.Substring(strRst.IndexOf("丢失=") + 3);
            //model.PingDiuShiLv = diuShiLv.Substring(diuShiLv.IndexOf("(")+1, diuShiLv.IndexOf("丢失)") - 2);
            //logHelper.Write(new Msg(strRst, MsgType.Error));
            //logHelper.Dispose();
            //Console.Write(strRst);
            //}
            #endregion
        }
    }
}
