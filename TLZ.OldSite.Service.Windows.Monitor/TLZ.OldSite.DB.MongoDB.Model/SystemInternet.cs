using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TLZ.OldSite.DB.MongoDB.Model
{
    /// <summary>
    /// 系统网络
    /// </summary>
    public class SystemInternet
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        /// <summary>
        /// 适配器名称
        /// </summary>
        public string InternetName { get; set; }

        /// <summary>
        /// 网卡类型
        /// </summary>
        public string InternetType { get; set; }

        /// <summary>
        /// 总下载字节
        /// </summary>
        public string ALLDownloadByte { get; set; }

        /// <summary>
        /// 总上传字节
        /// </summary>
        public string AllUpLoadByte { get; set; }

        /// <summary>
        /// 下载字节
        /// </summary>
        public string DownloadByte { get; set; }
        
        /// <summary>
        /// 上传字节
        /// </summary>
        public string UploadByte { get; set; }

        /// <summary>
        /// 当前时间
        /// </summary>
        public DateTime DateTimeNow { get; set; }
    }
}
