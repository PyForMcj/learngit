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
    /// 用户表
    /// </summary>
    public class Users
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string PassWord { get; set; }

        /// <summary>
        /// Persons表ID
        /// </summary>
        public string PersonID { get; set; }

        /// <summary>
        /// 当前时间
        /// </summary>
        public DateTime DateTimeNow { get; set; }

    }
}
