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
    /// 系统Cpu
    /// </summary>
    public class SystemCpu
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        private double _CpuUsageRate;

        private DateTime _DateTimeNow;

        /// <summary>
        /// 内存使用率
        /// </summary>
        public double CpuUsageRate
        {
            get { return _CpuUsageRate; }
            set { this._CpuUsageRate = value; }
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
