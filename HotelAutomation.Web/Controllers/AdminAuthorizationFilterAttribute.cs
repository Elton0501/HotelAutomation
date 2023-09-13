using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HotelAutomation.Web.Controllers
{
    public class AdminAuthorizationFilterAttribute : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Session["Admin"] == null || filterContext.HttpContext.Session["Captain"] == null && filterContext.HttpContext.Session["RCode"] == null)
            {
                filterContext.Result = new RedirectResult("/Error/ErrorPage");
            }
        }
    }
}