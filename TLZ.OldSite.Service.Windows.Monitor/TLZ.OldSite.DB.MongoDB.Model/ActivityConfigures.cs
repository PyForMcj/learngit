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
    /// 活动配置
    /// 比如：系统内存、系统CPU
    /// 判断他们的阈值、每隔多长时间、是否发送邮件等
    /// </summary>
    public class ActivityConfigures
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        private string _ActivityName;

        private string _ProcessName;

        private double _Threshold;

        private int _TntervalSecond;
         
        private bool _isEmail;
        
        private bool _isSMS;

        private DateTime _DateTimeNow;

        /// <summary>
        ///活动名
        ///与系统内存、系统CPU表名对应关系
        /// </summary>
        public string ActivityName
        {
            get { return _ActivityName; }
            set { this._ActivityName = value; }
        }

        /// <summary>
        ///进程名
        /// </summary>
        public string ProcessName
        {
            get { return _ProcessName; }
            set { this._ProcessName = value; }
        }

        /// <summary>
        ///阈值
        ///达到阈值报警
        /// </summary>
        public double Threshold
        {
            get { return _Threshold; }
            set { this._Threshold = value; }
        }

        /// <summary>
        ///时间(秒数)
        ///间隔多长时间获取一次
        /// </summary>
        public int TntervalSecond
        {
            get { return _TntervalSecond; }
            set { this._TntervalSecond = value; }
        }
        
        /// <summary>
        /// 邮件
        /// </summary>
        public bool isEmail
        {
            get { return _isEmail; }
            set { this._isEmail = value; }
        }
        
        /// <summary>
        /// 短信
        /// </summary>
        public bool isSMS
        {
            get { return _isSMS; }
            set { this._isSMS = value; }
        }
        
        /// <summary>
        ///当前时间
        /// </summary>
        public DateTime DateTimeNow
        {
            get { return _DateTimeNow; }
            set { this._DateTimeNow = value; }
        }
    }
}
