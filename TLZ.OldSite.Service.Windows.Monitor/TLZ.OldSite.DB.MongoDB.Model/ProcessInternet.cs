using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SharpPcap;

namespace TLZ.OldSite.DB.MongoDB.Model
{
    /// <summary>
    /// 进程网络
    /// </summary>
    public class ProcessInternet : IDisposable
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        /// <summary>
        /// 进程ID
        /// </summary>
        public int ProcessID { get; set; }

        /// <summary>
        /// 进程名
        /// </summary>
        public string ProcessName { get; set; }

        /// <summary>
        /// 每秒IO操作（不包含控制操作）读写数据的字节数(KB)
        /// </summary>
        public float IOOtherBytes { get; set; }

        /// <summary>
        /// 每秒IO操作数（不包括读写）(个数)
        /// </summary>
        public int IOOtherOperations { get; set; }

        /// <summary>
        /// 网络发送数据字节数
        /// </summary>
        public long NetSendBytes { get; set; }

        /// <summary>
        /// 网络接收数据字节数
        /// </summary>
        public long NetRecvBytes { get; set; }

        /// <summary>
        /// 网络数据总字节数
        /// </summary>
        public long NetTotalBytes { get; set; }

        /// <summary>
        /// 存储所有请求端口数据  cmd：netstat -ano
        /// </summary>
        public List<ICaptureDevice> dev = new List<ICaptureDevice>();

        /// <summary>
        /// 实现IDisposable的方法
        /// </summary>
        public void Dispose()
        {
            foreach (ICaptureDevice d in dev)
            {
                d.StopCapture();
                d.Close();
            }
        }
        public ProcessInternet ProcInfo { get; set; }

        public DateTime DateTimeNow { get; set; }
    }
}
