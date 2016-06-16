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
    /// 数据库和服务器配置
    /// </summary>
    public class DataBaseServerConfigures
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        private string _DataBaseName;

        private DateTime _DateTimeNow;

        private string _systemAddress;

        /// <summary>
        /// 对应数据库
        /// 直接采用服务器IP做数据库名称
        /// </summary>
        public string DataBaseName
        {
            get { return _DataBaseName; }
            set { _DataBaseName = value; }
        }

        /// <summary>
        /// 当前时间
        /// </summary>
        public DateTime DateTimeNow
        {
            get { return _DateTimeNow; }
            set { this._DateTimeNow = value; }
        }

        /// <summary>
        /// 服务器所在区域
        /// </summary>
        public string SystemAddress
        {
            get { return _systemAddress; }
            set { this._systemAddress = value; }
        }
    }
}
