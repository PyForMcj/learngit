using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using TLZ.MongoDB;
using TLZ.OldSite.DB.MongoDB.Model;

namespace TLZ.OldSite.WebSite.Admin.Models.PublicAction
{
    public static class CurrentUserInfo
    {
        public static string GetPersonName()
        {
            var currentUser = System.Web.HttpContext.Current.Session["CurrentUser"] as Users;
            List<IMongoQuery> queryList = new List<IMongoQuery>();
            IMongoQuery query = null;
            queryList.Add(Query<Persons>.EQ(item => item._id, currentUser.PersonID.Trim()));
            query = Query.And(queryList.ToArray());
            var model = MongoDBHelper.Get<Persons>(query, MongoConnType.Center);
            if (model==null)
            {
                throw new Exception("不存在此用户...");
            }
            return model.Name;
        }
    }
}