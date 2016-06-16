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
    /// 进程CPU
    /// </summary>
    public class ProcessCpu
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        private string _ProcessName;

        private double _ProcessUsageRate;

        private DateTime _DateTimeNow;

        /// <summary>
        /// 进程名
        /// </summary>
        public string ProcessName
        {
            get { return _ProcessName; }
            set { this._ProcessName = value; }
        }

        /// <summary>
        /// 进程cpu使用率
        /// </summary>
        public double ProcessUsageRate
        {
            get { return _ProcessUsageRate; }
            set { this._ProcessUsageRate = value; }
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
