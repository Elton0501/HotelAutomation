using DataBase;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace HotelAutomation.Web.Controllers
{
    public class NotificationactionController : Controller
    {
        [HttpPost]
        public ActionResult GetNotificationData()
        {
            var result = new List<Notification>();
            var rCode = HttpContext.Session["RCode"];
            var RCode = Convert.ToString(rCode);
            DateTime Current = DateTime.Now;
            using (var context = new ApplicationDbContext())
            {
                result = context.Notification.Where(x => x.RCode == RCode || x.isMultipleUser == true).ToList();
                result = result.Select(x =>
                {
                    x.TimeTrek = Current.Day > x.CreatedOn.Day ? Current.Day - x.CreatedOn.Day + " day ago" :
                    Current.Hour > x.CreatedOn.Hour ? Current.Hour - x.CreatedOn.Hour + " hours ago" :
                    Current.Minute > x.CreatedOn.Minute ? Current.Minute - x.CreatedOn.Minute + " minutes ago" : "";
                    return x;
                }).ToList();
            }
            var serializer = new JavaScriptSerializer();
            var data = serializer.Serialize(result);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public void SaveNotification(Notification notification)
        {
            if (notification.Message != "" && notification.Message != null)
            {
                using(var context = new ApplicationDbContext())
                {
                    context.Notification.Add(notification);
                    context.SaveChanges();
                    MyHub objNotifHub = new MyHub();
                    objNotifHub.NotificationBrodcaast(notification);
                }
            }
        }
        [HttpPost]
        public string DeleteNotificationData(int Id)
        {
            var result = "false";
            if (Id > 0)
            {
                using(var context = new ApplicationDbContext())
                {
                    var data = context.Notification.FirstOrDefault(x => x.Id == Id);
                    context.Notification.Remove(data);
                    context.SaveChanges();
                    result = "true";
                }
            }
            return result;
        }
    }
}