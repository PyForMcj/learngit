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
    /// 系统硬盘
    /// </summary>
    public class SystemHdd
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        private string _HddName;

        private double _AllSpace;

        private double _UseSpace;

        private double _SurplusSpace;

        private DateTime _DateTimeNow;

        /// <summary>
        /// 磁盘名称
        /// </summary>
        public string HddName
        {
            get { return _HddName; }
            set { this._HddName = value; }
        }

        /// <summary>
        /// 总空间
        /// </summary>
        public double AllSpace
        {
            get { return _AllSpace; }
            set { this._AllSpace = value; }
        }

        /// <summary>
        /// 已用空间
        /// </summary>
        public double UseSpace
        {
            get { return _UseSpace; }
            set { this._UseSpace = value; }
        }

        /// <summary>
        /// 剩余空间
        /// </summary>
        public double SurplusSpace
        {
            get { return _SurplusSpace; }
            set { this._SurplusSpace = value; }
        }

        /// <summary>
        /// 当前时间
        /// </summary>
        public DateTime DateTimeNow
        {
            get { return _DateTimeNow; }
            set { this._DateTimeNow = value; }
        }
    }
}
