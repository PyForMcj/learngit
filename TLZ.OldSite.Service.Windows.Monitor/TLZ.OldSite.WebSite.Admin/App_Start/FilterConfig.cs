using System.Web;
using System.Web.Mvc;

namespace TLZ.OldSite.WebSite.Admin
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}