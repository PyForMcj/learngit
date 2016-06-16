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
    /// 系统判断在线
    /// 是否能ping通
    /// </summary>
    public class SystemPing
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        private string _thisIP;

        private string _PingIP;

        private string _Status;

        private DateTime _DateTimeNow;

        private string _systemAddress;

        private double _pingByte;

        private double _pingDate;

        private double _pingTTL;

        private double _pingFaSong;

        private double _pingJieShou;

        private double _pingDiuShi;

        private string _pingDiuShiLv;

        /// <summary>
        /// 本机IP
        /// </summary>
        public string ThisIP
        {
            get { return _thisIP; }
            set { this._thisIP = value; }
        }

        /// <summary>
        /// 所pingIP
        /// </summary>
        public string PingIP
        {
            get { return _PingIP; }
            set { this._PingIP = value; }
        }

        
        /// <summary>
        /// 状态
        /// </summary>
        public string Status
        {
            get { return _Status; }
            set { this._Status = value; }
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

        /// <summary>
        /// ping字节
        /// </summary>
        public double PingByte
        {
            get { return _pingByte; }
            set { this._pingByte = value; }
        }

        /// <summary>
        /// ping时间
        /// </summary>
        public double PingDate
        {
            get { return _pingDate; }
            set { this._pingDate = value; }
        }

        /// <summary>
        /// pingTTL
        /// </summary>
        public double PingTTL
        {
            get { return _pingTTL; }
            set { this._pingTTL = value; }
        }

        /// <summary>
        /// ping已发送
        /// </summary>
        public double PingFaSong
        {
            get { return _pingFaSong; }
            set { this._pingFaSong = value; }
        }

        /// <summary>
        /// ping已接收
        /// </summary>
        public double PingJieShou
        {
            get { return _pingJieShou; }
            set { this._pingJieShou = value; }
        }

        /// <summary>
        /// ping丢失
        /// </summary>
        public double PingDiuShi
        {
            get { return _pingDiuShi; }
            set { this._pingDiuShi = value; }
        }

        /// <summary>
        /// ping丢失率
        /// </summary>
        public string PingDiuShiLv
        {
            get { return _pingDiuShiLv; }
            set { this._pingDiuShiLv = value; }
        }
    }
}
