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
    /// 系统内存
    /// </summary>
    public class SystemMemories
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        private double _MemoryUsageRate;

        private decimal _MemoryUsageGB;

        private decimal _MemoryUsageMB;

        private decimal _MemoryALLCountGB;

        private DateTime _DateTimeNow;

        /// <summary>
        /// 内存使用率
        /// </summary>
        public double MemoryUsageRate
        {
            get { return _MemoryUsageRate; }
            set { this._MemoryUsageRate = value; }
        }

        /// <summary>
        /// 内存使用情况GB
        /// </summary>
        public decimal MemoryUsageGB
        {
            get { return _MemoryUsageGB; }
            set { this._MemoryUsageGB = value; }
        }

        /// <summary>
        /// 内存使用情况MB
        /// </summary>
        public decimal MemoryUsageMB
        {
            get { return _MemoryUsageMB; }
            set { this._MemoryUsageMB = value; }
        }

        /// <summary>
        /// 内存总大小GB
        /// </summary>
        public decimal MemoryALLCountGB
        {
            get { return _MemoryALLCountGB; }
            set { this._MemoryALLCountGB = value; }
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
