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
    /// 进程内存
    /// </summary>
    public class ProcessMemories
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        private string _ProcessName;

        private decimal _ProcessSpecialUseKB;

        private decimal _ProcessWorkingSetKB;

        private decimal _ProcessSubmitKB;

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
        /// 资源监视器-内存-专用(KB)
        /// </summary>
        public decimal ProcessSpecialUseKB
        {
            get { return _ProcessSpecialUseKB; }
            set { this._ProcessSpecialUseKB = value; }
        }

        /// <summary>
        /// 资源监视器-内存-工作集(KB)
        /// </summary>
        public decimal ProcessWorkingSetKB
        {
            get { return _ProcessWorkingSetKB; }
            set { this._ProcessWorkingSetKB = value; }
        }

        /// <summary>
        /// 资源监视器-内存-提交(KB)
        /// </summary>
        public decimal ProcessSubmitKB
        {
            get { return _ProcessSubmitKB; }
            set { this._ProcessSubmitKB = value; }
        }

        /// <summary>
        ///该进程所用内存百分比（专用/总内存）
        ///暂时不确定是否这样计算正确
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
