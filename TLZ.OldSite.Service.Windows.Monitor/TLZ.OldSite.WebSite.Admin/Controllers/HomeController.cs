using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using TLZ.MongoDB;
using TLZ.OldSite.DB.MongoDB.Model;

namespace TLZ.OldSite.WebSite.Admin.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string userName, string passWord)
        {
            List<IMongoQuery> queryList = new List<IMongoQuery>();
            IMongoQuery query = null;
            queryList.Add(Query<Users>.EQ(item => item.UserName, userName.Trim()));
            queryList.Add(Query<Users>.EQ(item => item.PassWord, passWord.Trim()));
            query = Query.And(queryList.ToArray());

            var List = MongoDBHelper.Select<Users>(query, MongoConnType.Center);
            if (List.Count > 1)
            {
                ViewData["error"] = "系统中存在重复用户...";
                return View();
            }
            else if (List.Count == 1)
            {
                Session["CurrentUser"] = List.FirstOrDefault();
                return RedirectToAction("Index", "Default");
            }
            else
            {
                ViewData["error"] = "用户名或密码错误...";
                return View();
            }
        }

        public ActionResult LoginOut()
        {
            Session["CurrentUser"] = null;
            return RedirectToAction("Login", "Home");
        }
    }
}
