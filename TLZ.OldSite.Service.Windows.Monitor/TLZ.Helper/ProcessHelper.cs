using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Echevil;
using Microsoft.VisualBasic.Devices;
using SharpPcap;
using TLZ.OldSite.DB.MongoDB.Model;

namespace TLZ.Helper
{
    /// <summary>
    /// 获取操作系统任务管理器
    /// 信息的帮助类
    /// </summary>
    public class ProcessHelper
    {
        private readonly PerformanceCounter _cpu;
        private readonly ComputerInfo _cinf;
        private readonly string _ip;
        private readonly NetworkInterface[] _interfaceCard;
        private string _uploadInternet = "0";//上行默认数值
        private string _downloadInternet = "0";//下行默认数值
        public ProcessInternet _procInfo = null;
        public ProcessHelper()
        {
            _cpu = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            _cinf = new ComputerInfo();
            _ip = GetIPAddress();
            _interfaceCard = NetworkInterface.GetAllNetworkInterfaces();
            _procInfo = new ProcessInternet();
        }

        #region IP获取
        public string GetIPAddress()
        {
            try
            {
                //获取IP地址 
                string st = "";
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    if ((bool)mo["IPEnabled"] == true)
                    {
                        System.Array ar;
                        ar = (System.Array)(mo.Properties["IpAddress"].Value);
                        st = ar.GetValue(0).ToString();
                        break;
                    }
                }
                moc = null;
                mc = null;
                return st;
            }
            catch
            {
                return "unknow";
            }
            finally
            {
            }
        }
        #endregion

        #region 获取系统内存信息
        public SystemMemories GetSystemMemory()
        {
            var cinf = this._cinf;
            var used_mem = cinf.TotalPhysicalMemory - cinf.AvailablePhysicalMemory;
            var mb_mem = used_mem.ConvertBytes(2);
            var pmem = Convert.ToUInt64(mb_mem).ConvertBytes(1);
            var memoryUsageRate = Math.Round((double)(used_mem / Convert.ToDecimal(cinf.TotalPhysicalMemory) * 100), 0);
            var memoryALLCountGB = cinf.TotalPhysicalMemory.ConvertBytes(3);
            return new SystemMemories
            {
                MemoryUsageRate = memoryUsageRate,
                MemoryUsageGB = pmem,
                MemoryUsageMB = mb_mem,
                MemoryALLCountGB = memoryALLCountGB,
                DateTimeNow = DateTime.Now
            };
        }
        #endregion

        #region 获取系统CPU信息
        public SystemCpu GetSystemCpu()
        {
            var cpuUsageRate = Math.Round(this._cpu.NextValue(), 0);
            return new SystemCpu
            {
                CpuUsageRate = cpuUsageRate,
                DateTimeNow = DateTime.Now
            };
        }
        #endregion

        #region 获取系统硬盘
        public List<SystemHdd> GetSystemHdd()
        {
            List<SystemHdd> systemHdd = new List<SystemHdd>();
            List<SystemHddContext> listInfo = GetDiskListInfo();
            if (listInfo != null && listInfo.Count > 0)
            {
                foreach (SystemHddContext disk in listInfo)
                {
                    systemHdd.Add(new SystemHdd
                    {
                        HddName = disk.PartitionName,
                        AllSpace = ManagerDoubleValue(disk.SumSpace, 1),
                        UseSpace = ManagerDoubleValue(disk.SumSpace - disk.FreeSpace, 1),
                        SurplusSpace = ManagerDoubleValue(disk.FreeSpace, 1),
                        DateTimeNow = DateTime.Now
                    });
                }
            }
            return systemHdd;

        }
        private List<SystemHddContext> GetDiskListInfo()
        {
            List<SystemHddContext> list = null;
            //指定分区的容量信息
            try
            {
                SelectQuery selectQuery = new SelectQuery("select * from win32_logicaldisk");

                ManagementObjectSearcher searcher = new ManagementObjectSearcher(selectQuery);

                ManagementObjectCollection diskcollection = searcher.Get();
                if (diskcollection != null && diskcollection.Count > 0)
                {
                    list = new List<SystemHddContext>();
                    SystemHddContext harddisk = null;
                    foreach (ManagementObject disk in searcher.Get())
                    {
                        int nType = Convert.ToInt32(disk["DriveType"]);
                        if (nType != Convert.ToInt32(DriveType.Fixed))
                        {
                            continue;
                        }
                        else
                        {
                            harddisk = new SystemHddContext();
                            harddisk.FreeSpace = Convert.ToDouble(disk["FreeSpace"]) / (1024 * 1024 * 1024);
                            harddisk.SumSpace = Convert.ToDouble(disk["Size"]) / (1024 * 1024 * 1024);
                            harddisk.PartitionName = disk["DeviceID"].ToString();
                            list.Add(harddisk);
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
            return list;
        }
        public double ManagerDoubleValue(double _value, int Length)
        {
            if (Length < 0)
            {
                Length = 0;
            }
            return System.Math.Round(_value, Length);
        }
        #endregion

        #region 获取系统PING

        public SystemPing GetSystemPing(string pingIP)
        {
            var model = new SystemPing() { };
            model.ThisIP = this._ip;
            model.PingIP = pingIP;
            model.DateTimeNow = DateTime.Now;

            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            p.StandardInput.WriteLine("ping -n 1 " + pingIP);
            p.StandardInput.WriteLine("exit");
            string strRst = p.StandardOutput.ReadToEnd();
            strRst = Regex.Replace(strRst, @"\s", "");
            if (strRst.IndexOf("Ping统计信息:") != -1)
            {
                model.Status = "请求成功";
                string ziJie = strRst.Substring(strRst.IndexOf("字节=") + 3);
                model.PingByte = double.Parse(ziJie.Substring(0, ziJie.IndexOf("时间")));
                string shiJian = strRst.Substring(strRst.IndexOf("时间") + 3);
                model.PingDate = double.Parse(shiJian.Substring(0, shiJian.IndexOf("TTL=") - 2));
                string ttl = strRst.Substring(strRst.IndexOf("TTL=") + 4);
                model.PingTTL = double.Parse(ttl.Substring(0, ttl.IndexOf(pingIP + "的Ping统计信息:")));
                string yiFaSong = strRst.Substring(strRst.IndexOf("已发送=") + 4);
                model.PingFaSong = double.Parse(yiFaSong.Substring(0, yiFaSong.IndexOf("已接收=") - 1));
                string yiJieShou = strRst.Substring(strRst.IndexOf("已接收=") + 4);
                model.PingJieShou = double.Parse(yiJieShou.Substring(0, yiJieShou.IndexOf("丢失=") - 1));
                string diuShi = strRst.Substring(strRst.IndexOf("丢失=") + 3);
                model.PingDiuShi = double.Parse(diuShi.Substring(0, diuShi.IndexOf("(")));
                string diuShiLv = strRst.Substring(strRst.IndexOf("丢失=") + 3);
                model.PingDiuShiLv = diuShiLv.Substring(diuShiLv.IndexOf("(") + 1, diuShiLv.IndexOf("丢失)") - 2);
            }
            else if (strRst.IndexOf("请求超时。") != -1)
            {
                model.Status = "请求超时";
            }
            else if (strRst.IndexOf("请求找不到主机") != -1)
            {
                model.Status = "请求找不到主机";
            }
            else if (strRst.IndexOf("不知名主机") != -1)
            {
                model.Status = "请求无法解析主机";
            }
            else
            {
                model.Status = strRst;
            }
            p.Close();
            return model;
        }
        #endregion

        #region 获取系统网络

        #region 方法一 不准确
        /// <summary>
        /// 获取系统网络流量情况：
        /// 上传、下载
        /// 此功能与网卡有关，目前暂不确定是否可行
        /// </summary>
        public SystemInternet GetSystemInternet()
        {
            //网卡不确定有几个，默认[0]（本地连接）
            //可在：控制面板-网络和 Internet-网络连接 中查看网卡
            NetworkInterface nic = _interfaceCard[0];
            IPv4InterfaceStatistics interfaceStats = nic.GetIPv4Statistics();

            int bytesSentSpeed = (int)(interfaceStats.BytesSent - double.Parse(_uploadInternet)) / 1024;
            int bytesReceivedSpeed = (int)(interfaceStats.BytesReceived - double.Parse(_downloadInternet)) / 1024;

            _uploadInternet = interfaceStats.BytesSent.ToString();
            _downloadInternet = interfaceStats.BytesReceived.ToString();

            return new SystemInternet
            {
                InternetName = nic.Name,
                InternetType = nic.NetworkInterfaceType.ToString(),
                ALLDownloadByte = interfaceStats.BytesReceived.ToString(),
                AllUpLoadByte = interfaceStats.BytesSent.ToString(),
                UploadByte = bytesSentSpeed.ToString(),
                DownloadByte = bytesReceivedSpeed.ToString(),
                DateTimeNow = DateTime.Now
            };
        }
        #endregion

        #region 方法二 已测试比较准确
        //NetInfo netinfo = new NetInfo();
        //string strNetName;
        //PerformanceCounter pc;
        //NetInfo.NetPerformanceStruct netInfoStruct;



        //List<NetInfo.NetInfoBaseStruct> lNetInfoBase = netinfo.GetNetInfoBase();

        //strNetName = lNetInfoBase[0].NetName;

        //#region 输出
        //Console.Write("网卡名称：" + lNetInfoBase[0].NetName + "\n");
        //Console.Write("IP地址IPV4：" + lNetInfoBase[0].IPAddress[0] + "\n");
        //Console.Write("IP地址IPV6：" + lNetInfoBase[0].IPAddress[1] + "\n");
        //Console.Write("MAC地址：" + lNetInfoBase[0].MACAddress + "\n");
        //Console.Write("默认网关：" + lNetInfoBase[0].DefaultIPGateway + "\n");
        //Console.Write("子网掩码：" + lNetInfoBase[0].IPSubnet + "\n");
        //#endregion
        //while (true)
        //{
        //    netInfoStruct = netinfo.GetNetPerformance(strNetName);
        //    Console.Write("--------------------------开始---------------------------\n");
        //    Console.Write("每秒字节总数：" + netInfoStruct.BytesTotal.ToString() + "\n");
        //    Console.Write("每秒发送字节数：" + netInfoStruct.BytesSent.ToString() + "\n");
        //    Console.Write("每秒接收字节数：" + netInfoStruct.BytesReceived.ToString() + "\n");
        //    Console.Write("每秒包总数：" + netInfoStruct.PacketsTotal.ToString() + "\n");
        //    Console.Write("每秒包发送数：" + netInfoStruct.PacketsSent.ToString() + "\n");
        //    Console.Write("每秒发送单播包数：" + netInfoStruct.PacketsSentUnicast.ToString() + "\n");
        //    Console.Write("每秒发送非单播包数：" + netInfoStruct.PacketsSentNonUnicast.ToString() + "\n");
        //    Console.Write("丢弃的发送包数：" + netInfoStruct.PacketsSentDiscarded.ToString() + "\n");
        //    Console.Write("错误的发送包数：" + netInfoStruct.PacketsSentErrors.ToString() + "\n");
        //    Console.Write("每秒包接收数：" + netInfoStruct.PacketsReceived.ToString() + "\n");
        //    Console.Write("每秒单播包接收数：" + netInfoStruct.PacketsReceivedUnicast.ToString() + "\n");
        //    Console.Write("每秒非单播包接收总数：" + netInfoStruct.PacketsReceivedNonUnicast.ToString() + "\n");
        //    Console.Write("丢弃的接收包数：" + netInfoStruct.PacketsReceivedDiscarded.ToString() + "\n");
        //    Console.Write("错误的接收包数：" + netInfoStruct.PacketsReceivedErrors.ToString() + "\n");
        //    Console.Write("--------------------------结束---------------------------\n");
        //    Thread.Sleep(1000);
        //}
        #endregion

        #region 方法三 已测试比较准确
        public SystemInternet GetSystemInternet(NetworkAdapter[] _adapters)
        {
            return new SystemInternet
            {
                InternetName = _adapters[0].Name,
                UploadByte = (Math.Round(_adapters[0].UploadSpeedKbps, 2)).ToString(),
                DownloadByte = (Math.Round(_adapters[0].DownloadSpeedKbps, 2)).ToString(),
                DateTimeNow = DateTime.Now
            };
        }
        #endregion
        #endregion

        #region 获取进程内存
        public ProcessMemories GetProcessMemories(string _processName)
        {
            if (Process.GetProcessesByName(_processName).Length > 0)
            {
                using (var process = Process.GetProcessesByName(_processName)[0])
                {
                    var p1 = new PerformanceCounter("Process", "Working Set - Private", _processName);
                    var p3 = new PerformanceCounter("Process", "Private Bytes", _processName);
                    return new ProcessMemories
                    {
                        ProcessName = _processName,
                        ProcessSpecialUseKB = decimal.Round((Math.Round((decimal)p1.NextValue(), 0) / 1024) / 1000, 3),
                        ProcessWorkingSetKB = decimal.Round((Math.Round((decimal)process.WorkingSet64 / 1024, 0)) / 1000, 3),
                        ProcessSubmitKB = decimal.Round((Math.Round((decimal)p3.NextValue(), 0) / 1024) / 1000, 3),
                        ProcessUsageRate = 0,
                        DateTimeNow = DateTime.Now
                    };
                }
            }
            else
            {
                return new ProcessMemories();
            }
        }
        #endregion

        #region 获取进程CPU
        /// <summary>
        /// 获取进程CPU
        /// </summary>
        /// <param name="_processName">进程名称</param>
        /// <returns>ProcessCpu</returns>
        public ProcessCpu GetProcessCpu(string _processName, int interval)
        {
            if (Process.GetProcessesByName(_processName).Length > 0)
            {
                using (var pro = Process.GetProcessesByName(_processName)[0])
                {
                    //上次记录的CPU时间
                    var prevCpuTime = TimeSpan.Zero;
                    //当前时间
                    var curTime = pro.TotalProcessorTime;
                    //间隔时间内的CPU运行时间除以逻辑CPU数量
                    var value = (curTime - prevCpuTime).TotalMilliseconds / interval / Environment.ProcessorCount * 100;
                    prevCpuTime = curTime;
                    return new ProcessCpu
                    {
                        ProcessName = _processName,
                        ProcessUsageRate = value,
                        DateTimeNow = DateTime.Now
                    };
                }
            }
            else
            {
                return new ProcessCpu();
            }
        }
        #endregion

        #region 获取进程网络
        /// <summary>
        /// 进程网络
        /// </summary>
        /// <param name="_processName">进程名称</param>
        /// <returns>ProcessInternet</returns>
        public ProcessInternet GetProcessInternet(string _processName, int tntervalSecond)
        {
            if (Process.GetProcessesByName(_processName).Length > 0)
            {
                using (var proModel = Process.GetProcessesByName(_processName)[0])
                {
                    //进程id
                    int pid = proModel.Id;
                    //存放进程使用的端口号链表
                    List<int> ports = new List<int>();

                    #region 获取指定进程对应端口号
                    Process pro = new Process();
                    pro.StartInfo.FileName = "cmd.exe";
                    pro.StartInfo.UseShellExecute = false;
                    pro.StartInfo.RedirectStandardInput = true;
                    pro.StartInfo.RedirectStandardOutput = true;
                    pro.StartInfo.RedirectStandardError = true;
                    pro.StartInfo.CreateNoWindow = true;
                    pro.Start();
                    pro.StandardInput.WriteLine("netstat -ano");
                    pro.StandardInput.WriteLine("exit");
                    Regex reg = new Regex("\\s+", RegexOptions.Compiled);
                    string line = null;
                    ports.Clear();
                    while ((line = pro.StandardOutput.ReadLine()) != null)
                    {
                        line = line.Trim();
                        if (line.StartsWith("TCP", StringComparison.OrdinalIgnoreCase))
                        {
                            line = reg.Replace(line, ",");
                            string[] arr = line.Split(',');
                            if (arr[4] == pid.ToString())
                            {
                                string soc = arr[1];
                                int pos = soc.LastIndexOf(':');
                                int pot = int.Parse(soc.Substring(pos + 1));
                                ports.Add(pot);
                            }
                        }
                        else if (line.StartsWith("UDP", StringComparison.OrdinalIgnoreCase))
                        {
                            line = reg.Replace(line, ",");
                            string[] arr = line.Split(',');
                            if (arr[3] == pid.ToString())
                            {
                                string soc = arr[1];
                                int pos = soc.LastIndexOf(':');
                                int pot = int.Parse(soc.Substring(pos + 1));
                                ports.Add(pot);
                            }
                        }
                    }
                    pro.Close();
                    #endregion

                    #region 获取本机IP地址
                    //获取本机IP地址
                    IPAddress[] addrList = Dns.GetHostByName(Dns.GetHostName()).AddressList;
                    string IP = addrList[0].ToString();
                    //获取本机网络设备
                    var devices = CaptureDeviceList.Instance;
                    int count = devices.Count;
                    if (count < 1)
                    {
                        throw new Exception("本机网络设备不存在");
                    }
                    #endregion

                    #region 开始抓包
                    //开始抓包
                    for (int i = 0; i < count; ++i)
                    {
                        for (int j = 0; j < ports.Count; ++j)
                        {
                            CaptureFlowRecv(IP, ports[j], i);
                            CaptureFlowSend(IP, ports[j], i);
                        }
                    }
                    #endregion

                    //long NetTotalBytes = _procInfo.NetTotalBytes;
                    //long NetSendBytes = _procInfo.NetSendBytes;
                    //long NetRecvBytes = _procInfo.NetRecvBytes;
                    RefershInfo(tntervalSecond);
                    _procInfo.Dispose();
                    return new ProcessInternet
                    {
                        ProcessID = pid,
                        ProcessName = proModel.ProcessName,
                        NetSendBytes = _procInfo.NetSendBytes,
                        NetRecvBytes = _procInfo.NetRecvBytes,
                        NetTotalBytes = _procInfo.NetTotalBytes,
                        DateTimeNow = DateTime.Now
                    };

                    //最后要记得调用Dispose方法停止抓包并关闭设备

                }
            }
            else
            {
                return new ProcessInternet();
            }
        }
        public void RefershInfo(int tntervalSecond)
        {
            _procInfo.NetRecvBytes = 0;
            _procInfo.NetSendBytes = 0;
            _procInfo.NetTotalBytes = 0;
            Thread.Sleep(tntervalSecond);
            _procInfo.NetTotalBytes = _procInfo.NetRecvBytes + _procInfo.NetSendBytes;
        }
        public void CaptureFlowSend(string IP, int portID, int deviceID)
        {
            ICaptureDevice device = (ICaptureDevice)CaptureDeviceList.New()[deviceID];

            device.OnPacketArrival += new PacketArrivalEventHandler(device_OnPacketArrivalSend);

            int readTimeoutMilliseconds = 1000;
            device.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);

            string filter = "src host " + IP + " and src port " + portID;
            device.Filter = filter;
            device.StartCapture();
            _procInfo.dev.Add(device);
        }
        public void CaptureFlowRecv(string IP, int portID, int deviceID)
        {
            ICaptureDevice device = CaptureDeviceList.New()[deviceID];
            device.OnPacketArrival += new PacketArrivalEventHandler(device_OnPacketArrivalRecv);

            int readTimeoutMilliseconds = 1000;
            device.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);

            string filter = "dst host " + IP + " and dst port " + portID;
            device.Filter = filter;
            device.StartCapture();
            _procInfo.dev.Add(device);
        }
        private void device_OnPacketArrivalSend(object sender, CaptureEventArgs e)
        {
            var len = e.Packet.Data.Length;
            _procInfo.NetSendBytes += len;
        }
        private void device_OnPacketArrivalRecv(object sender, CaptureEventArgs e)
        {
            var len = e.Packet.Data.Length;
            _procInfo.NetRecvBytes += len;
        }
        #endregion
    }

    /// <summary>
    /// 获取系统网络
    /// 另外一种方法，可能比上面那个准确点
    /// </summary>
    public class NetInfo
    {
        NetPerformanceStruct netPerformanceStruct;

        //定义PerformanceCounter
        PerformanceCounter pcBytesTotal = new PerformanceCounter();
        PerformanceCounter pcBytesSent = new PerformanceCounter();
        PerformanceCounter pcBytesReceived = new PerformanceCounter();
        PerformanceCounter pcPacketsTotal = new PerformanceCounter();
        PerformanceCounter pcPacketsSent = new PerformanceCounter();
        PerformanceCounter pcPacketsSentUnicast = new PerformanceCounter();
        PerformanceCounter pcPacketsSentNonUnicast = new PerformanceCounter();
        PerformanceCounter pcPacketsSentDiscarded = new PerformanceCounter();
        PerformanceCounter pcPacketsSentErrors = new PerformanceCounter();
        PerformanceCounter pcPacketsReceived = new PerformanceCounter();
        PerformanceCounter pcPacketsReceivedUnicast = new PerformanceCounter();
        PerformanceCounter pcPacketsReceivedNonUnicast = new PerformanceCounter();
        PerformanceCounter pcPacketsReceivedDiscarded = new PerformanceCounter();
        PerformanceCounter pcPacketsReceivedErrors = new PerformanceCounter();

        //构造函数
        public NetInfo()
        {

        }


        //网络基础信息结构体
        public struct NetInfoBaseStruct
        {
            public string NetName;  //网络名称
            public string[] IPAddress;  //IP地址，包括IPv4和IPv6
            public string MACAddress;  //MAC地址
            public string IPSubnet;  //子网掩码
            public string DefaultIPGateway;  //默认网关
        }


        //网络性能结构体
        public struct NetPerformanceStruct
        {
            //字节
            public float BytesTotal;  //每秒总字节数
            public float BytesSent;  //每秒发送字节数
            public float BytesReceived;  //每秒发送字节数

            //包
            public float PacketsTotal;  //每秒总包数

            //包发送
            public float PacketsSent;  //每秒发送包数
            public float PacketsSentUnicast;  //每秒发送单播包数
            public float PacketsSentNonUnicast;  //每秒发送非单播包数
            public float PacketsSentDiscarded;  //被丢弃的发送包数
            public float PacketsSentErrors;  //错误的发送包数


            //包接收
            public float PacketsReceived;  //每秒接收包数
            public float PacketsReceivedUnicast;  //每秒接收单播包数
            public float PacketsReceivedNonUnicast;  //每秒接收非单播包数
            public float PacketsReceivedDiscarded;  //被丢弃的接收包数
            public float PacketsReceivedErrors;  //错误的接收包数



        }


        //获取网络基本信息
        /**/
        /// <summary>
        /// 获取网络基本信息
        /// </summary>
        /// <returns>包含网络基本信息结构体的列表</returns>
        public List<NetInfoBaseStruct> GetNetInfoBase()
        {
            List<NetInfoBaseStruct> lNetInfo = new List<NetInfoBaseStruct>();
            NetInfoBaseStruct netInfoBaseStruct;


            ManagementObjectSearcher query =
                            new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration WHERE IPEnabled = 'TRUE'");
            ManagementObjectCollection moCollection = query.Get();

            //遍历
            foreach (ManagementObject mObject in moCollection)
            {
                try
                {
                    netInfoBaseStruct = new NetInfoBaseStruct();
                    netInfoBaseStruct.NetName = mObject["Description"].ToString();
                    netInfoBaseStruct.IPAddress = (string[])mObject["IPAddress"];
                    netInfoBaseStruct.MACAddress = mObject["MACAddress"].ToString();
                    //DefaultIPGateway
                    if (mObject["DefaultIPGateway"] == null)  //如果默认网关只有一个则返回
                    {
                        netInfoBaseStruct.DefaultIPGateway = "";
                    }
                    else   //否则返回默认网关的数组(注：这里为了简单起见，只返回第一个网关)
                    {
                        netInfoBaseStruct.DefaultIPGateway = ((string[])mObject["DefaultIPGateway"])[0];
                    }

                    //IPSubnet
                    if (mObject["IPSubnet"] == null)
                    {
                        netInfoBaseStruct.IPSubnet = "";
                    }
                    else
                    {
                        netInfoBaseStruct.IPSubnet = ((string[])mObject["IPSubnet"])[0];
                    }

                    lNetInfo.Add(netInfoBaseStruct);

                }
                catch
                {
                    //continue;
                }

            }

            return lNetInfo;
        }

        //GetNetPerformance
        public NetPerformanceStruct GetNetPerformance(string NetName)
        {

            netPerformanceStruct = new NetPerformanceStruct();

            //定义CategoryName
            pcBytesTotal.CategoryName = "Network Interface";
            pcBytesSent.CategoryName = "Network Interface";
            pcBytesReceived.CategoryName = "Network Interface";
            pcPacketsTotal.CategoryName = "Network Interface";
            pcPacketsSent.CategoryName = "Network Interface";
            pcPacketsSentUnicast.CategoryName = "Network Interface";
            pcPacketsSentNonUnicast.CategoryName = "Network Interface";
            pcPacketsSentDiscarded.CategoryName = "Network Interface";
            pcPacketsSentErrors.CategoryName = "Network Interface";
            pcPacketsReceived.CategoryName = "Network Interface";
            pcPacketsReceivedUnicast.CategoryName = "Network Interface";
            pcPacketsReceivedNonUnicast.CategoryName = "Network Interface";
            pcPacketsReceivedDiscarded.CategoryName = "Network Interface";
            pcPacketsReceivedErrors.CategoryName = "Network Interface";

            //本次改进主要部分，主要思路是通过PerformanceCounterCategory这个类来获取Network Interface下
            //的所有实例(即网卡名称)，先将这个网卡名称处理掉所有的特殊字符，然后与传递进来的网卡名称(也处理掉特殊字符)
            //进行比较，如果相同，就将符合格式的实例名称赋给网卡名称，接下去就是获取性能信息。。。
            string[] instanceNames;
            PerformanceCounterCategory mycat = new PerformanceCounterCategory("Network Interface");
            try
            {
                instanceNames = mycat.GetInstanceNames();
                string tmp;
                string tmpNormal;

                for (int i = 0; i <= instanceNames.Length; i++)
                {
                    tmp = Regex.Replace(instanceNames[i], @"[^a-zA-Z\d]", "");
                    tmpNormal = Regex.Replace(NetName, @"[^a-zA-Z\d]", "");

                    if (tmp == tmpNormal)
                    {
                        NetName = instanceNames[i];
                    }
                }
            }
            catch { }

            //定义InstanceName
            pcBytesTotal.InstanceName = NetName;
            pcBytesSent.InstanceName = NetName;
            pcBytesReceived.InstanceName = NetName;
            pcPacketsTotal.InstanceName = NetName;
            pcPacketsSent.InstanceName = NetName;
            pcPacketsSentUnicast.InstanceName = NetName;
            pcPacketsSentNonUnicast.InstanceName = NetName;
            pcPacketsSentDiscarded.InstanceName = NetName;
            pcPacketsSentErrors.InstanceName = NetName;
            pcPacketsReceived.InstanceName = NetName;
            pcPacketsReceivedUnicast.InstanceName = NetName;
            pcPacketsReceivedNonUnicast.InstanceName = NetName;
            pcPacketsReceivedDiscarded.InstanceName = NetName;
            pcPacketsReceivedErrors.InstanceName = NetName;

            //定义CounterName
            pcBytesTotal.CounterName = "Bytes Total/sec";
            pcBytesSent.CounterName = "Bytes Sent/sec";
            pcBytesReceived.CounterName = "Bytes Received/sec";
            pcPacketsTotal.CounterName = "Packets/sec";
            pcPacketsSent.CounterName = "Packets Sent/sec";
            pcPacketsSentUnicast.CounterName = "Packets Sent Unicast/sec";
            pcPacketsSentNonUnicast.CounterName = "Packets Sent Non-Unicast/sec";
            pcPacketsSentDiscarded.CounterName = "Packets Outbound Discarded";
            pcPacketsSentErrors.CounterName = "Packets Outbound Errors";
            pcPacketsReceived.CounterName = "Packets Received/sec";
            pcPacketsReceivedUnicast.CounterName = "Packets Received Unicast/sec";
            pcPacketsReceivedNonUnicast.CounterName = "Packets Received Non-Unicast/sec";
            pcPacketsReceivedDiscarded.CounterName = "Packets Received Discarded";
            pcPacketsReceivedErrors.CounterName = "Packets Received Errors";

            try
            {
                //为结构体赋值
                netPerformanceStruct.BytesTotal = pcBytesTotal.NextValue();
                netPerformanceStruct.BytesSent = pcBytesSent.NextValue();
                netPerformanceStruct.BytesReceived = pcBytesReceived.NextValue();
                netPerformanceStruct.PacketsTotal = pcPacketsTotal.NextValue();
                netPerformanceStruct.PacketsSent = pcPacketsSent.NextValue();
                netPerformanceStruct.PacketsSentUnicast = pcPacketsSentUnicast.NextValue();
                netPerformanceStruct.PacketsSentNonUnicast = pcPacketsSentNonUnicast.NextValue();
                netPerformanceStruct.PacketsSentDiscarded = pcPacketsSentDiscarded.NextValue();
                netPerformanceStruct.PacketsSentErrors = pcPacketsSentErrors.NextValue();
                netPerformanceStruct.PacketsReceived = pcPacketsReceived.NextValue();
                netPerformanceStruct.PacketsReceivedUnicast = pcPacketsReceivedUnicast.NextValue();
                netPerformanceStruct.PacketsReceivedNonUnicast = pcPacketsReceivedNonUnicast.NextValue();
                netPerformanceStruct.PacketsReceivedDiscarded = pcPacketsReceivedDiscarded.NextValue();
                netPerformanceStruct.PacketsReceivedErrors = pcPacketsReceivedErrors.NextValue();
            }
            catch
            {

            }

            return netPerformanceStruct;
        }
    }

}
