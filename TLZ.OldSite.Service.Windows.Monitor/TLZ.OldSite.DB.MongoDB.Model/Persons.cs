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
    /// 人员表
    /// </summary>
    public class Persons
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        /// <summary>
        /// 人员名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 当前时间
        /// </summary>
        public DateTime DateTimeNow { get; set; }
    }
}
