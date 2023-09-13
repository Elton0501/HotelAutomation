using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HotelAutomation.Web.Controllers
{
    public class SuperAdminAuthorizationFilterAttribute : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Session["SuperAdmin"] == null)
            {
                filterContext.Result = new RedirectResult("/Error/ErrorPage");
            }
        }
    }
}