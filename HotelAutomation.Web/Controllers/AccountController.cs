using Models;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using ViewModels;

namespace HotelAutomation.Web.Controllers
{
    public class AccountController : Controller
    {
        APIController API = new APIController(); 
        // GET: Account
        public ActionResult Login(int? TCode)
        {
            try
            {
                PrintApiController printApi = new PrintApiController();
                RestViewModel model = new RestViewModel();
                if (Request.Cookies[Constant.WebCookie] != null)
                {
                    HttpCookie oldcookie = new HttpCookie(Constant.WebCookie);
                    oldcookie.Expires = DateTime.Now.AddDays(-1);
                    Response.Cookies.Add(oldcookie);
                }
                 //Uri link = new Uri("https://Testingone.bfoodie.in/Account/Login/?TCode=" + 2);
                Uri link = new Uri(HttpContext.Request.Url.AbsoluteUri);
                var rName = link.Host.Substring(0, link.Host.LastIndexOf(Constant.WebHost));
                var restaurant = RestaurantService.Instance.GetRestaurantByName(rName);
                if (restaurant == null)
                {
                    return RedirectToAction("ErrorPage", "Error", new { CloseWeb = false, maintainance = false, NotFound = true });
                }
                //find table id in url
                var Params = link.ParseQueryString();
                var tCode = link.ParseQueryString().Get("TCode");
                TCode = tCode != null ? Convert.ToInt32(tCode) : Convert.ToInt32(Constant.HomeDelivery);
                model.RLogo = restaurant.Img;
                model.TCode = TCode.ToString();
                model.RCode = restaurant.Id.ToString();
                model.isReserved = TableService.Instance.CheckTableAvailByRCodeTcode(restaurant.Id.ToString(),TCode.ToString());
                return View(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Error");
            }
        }
        [HttpPost]
        public ActionResult Login(string Mobile, string TCode, string RCode)
        {
            try
            {
                ResultModel model = new ResultModel();
                model.Result = false;
                if (Mobile == null && Mobile.Length != 10 || TCode == string.Empty || TCode == null
                    || RCode == string.Empty || RCode == null)
                {
                    return RedirectToAction("ErrorPage", "Error", new { CloseWeb = false, maintainance = false, NotFound = true });
                }
                var user = AccountService.Instance.saveUserDetails("", Mobile);
                if (user != null)
                {
                    bool IsVerify = RestaurantService.Instance.GetRestaurantByID(RCode).IsOtpVerification;
                    if (IsVerify == true)
                    {
                        var Details = API.OtpGenrate(user.MobileNumber);
                        return Json(new { Result = Details.Result, Status = Details.Status, Value = Mobile, Deatils = Details.Details, isOtp = true });
                    }
                    else
                    {
                        HttpCookie cookie = new HttpCookie(Constant.WebCookie);
                        cookie.Expires = DateTime.Now.AddHours(6);
                        var cookiedata = "UId" + Mobile + "," + "TCode" + TCode + "," + "RCode" + RCode;
                        cookie.Value = HelperService.Instance.Encrypt(cookiedata);
                        Response.Cookies.Set(cookie);
                        if (TCode != Constant.TakeAway && TCode != Constant.HomeDelivery)
                        {
                            var saveUserTable = TableService.Instance.BookTableByQrCode(Mobile, TCode, RCode);
                            if (saveUserTable == true)
                            {
                                var notification = new Notification();
                                notification.CreatedOn = DateTime.Now;
                                notification.isMultipleUser = false;
                                notification.RCode = RCode;
                                notification.Status = Constant.NotificationConstant.unread;
                                var birthdayTodaymsg = UserService.Instance.checkBirthdayMessage(Mobile);
                                notification.Message = string.Format(Constant.NotificationConstant.AddNewUser, TCode, Mobile, birthdayTodaymsg);
                                var notificationAction = new NotificationactionController();
                                notificationAction.SaveNotification(notification);
                            }
                        }
                        return Json(new { Result = true, isOtp = true }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new { Result = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Error",new { CloseWeb = false, maintainance = true,NotFound = false});
            }
        }
        public ActionResult OtpVerification(string UserIdentity, string Details, bool mNumber,string RCode, string TCode)
        {
            try
            {
                var result = new OTPModel();
                if (UserIdentity != null && Details != null)
                {
                    result.UserIdentity = UserIdentity;
                    result.Details = Details;
                    result.MblNumber = mNumber;
                    result.RCode = RCode;
                    result.TCode = TCode;
                }
                return View(result);
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "Error", new { maintainance = true, NotFound = false });
            }
        }
        [HttpPost]
        public ActionResult OtpVerification(string Otp, string UserIdentity, string OtpDetails, bool mNumber, string RCode, string TCode)
        {
            try
            {
                bool result = false;
                bool request = true;
                var VerifyDetail = new OTPModel();
                VerifyDetail.Otp = Otp;
                VerifyDetail.Details = OtpDetails;
                VerifyDetail.UserIdentity = UserIdentity;
                VerifyDetail.MblNumber = mNumber;
                if (VerifyDetail.UserIdentity != null && VerifyDetail.Details != null)
                {
                    var Details = new OTPModel();
                    if (mNumber == true)
                    {Details = API.OtpVerify(VerifyDetail);}
                    else{Details = AccountService.Instance.OtpVerify(VerifyDetail);}
                    if (Details.Status == "Success")
                    {
                        result = true;
                        request = false;
                        HttpCookie cookie = new HttpCookie(Constant.WebCookie);
                        cookie.Expires = DateTime.Now.AddHours(6);
                        var cookiedata = "UId" + UserIdentity + "," + "TCode" + TCode + "," + "RCode" + RCode;
                        cookie.Value = HelperService.Instance.Encrypt(cookiedata);
                        Response.Cookies.Set(cookie);
                        if (TCode.ToString() != Constant.TakeAway && TCode.ToString() != Constant.HomeDelivery)
                        {
                            TableService.Instance.BookTableByQrCode(UserIdentity, TCode.ToString(), RCode);
                        }
                    }
                    else{result = false;}
                }
                else{result = false;}
                return Json(new { status = result, Request = request, TCode = TCode,  });
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "Error");
            }
        }
    }
}