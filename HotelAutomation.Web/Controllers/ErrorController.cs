using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ViewModels;

namespace HotelAutomation.Web.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult ErrorPage(bool CloseWeb = true, bool maintainance = false, bool NotFound = false)
        {
            if (Request.Cookies[Constant.WebCookie] != null)
            {
                HttpCookie oldcookie = new HttpCookie(Constant.WebCookie);
                oldcookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(oldcookie);
            }
            ViewBag.Maintainance = maintainance;
            ViewBag.NotFound = NotFound;
            ViewBag.CloseWeb = CloseWeb;
            return View();
        }
    }
}