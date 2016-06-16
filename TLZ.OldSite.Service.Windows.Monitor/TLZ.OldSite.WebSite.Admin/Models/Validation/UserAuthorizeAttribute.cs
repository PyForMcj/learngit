using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace TLZ.OldSite.WebSite.Admin.Models
{
    /// <summary>
    /// 判断登录验证
    /// </summary>
    public class UserValidateAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.Current.Session["CurrentUser"]==null)
            {
                filterContext.Result = new RedirectToRouteResult("Default", new RouteValueDictionary(new { controller = "Home", action = "Login" }));
                //filterContext.Result = new RedirectResult("~/Home/Login");
            }
        }
    }
}