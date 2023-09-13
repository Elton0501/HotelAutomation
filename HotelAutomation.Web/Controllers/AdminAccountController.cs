using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ViewModels;

namespace HotelAutomation.Web.Controllers
{
    public class AdminAccountController : Controller
    {
        HelperController helperController = new HelperController();
        // GET: AdminAccount
        [Route("bfadminlogin")]
        public ActionResult SuperAdminLogin()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SuperAdminLogin(superAdminLogin model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var result = SuperAdminService.Instance.Login(model);
            HttpContext.Session["Captain"] = "";
            HttpContext.Session["SuperAdmin"] = "";
            HttpContext.Session["Admin"] = "";
            HttpContext.Session["RCode"] = "";
            if (result.IsSuperAdmin == true)
            {
                HttpContext.Session["SuperAdmin"] = model.Email;
                return RedirectToAction("Index", "SuperAdmin");
            }
            else if (result.IsAdmin == true && result.RCode > 0)
            {
                HttpContext.Session["Admin"] = model.Email;
                HttpContext.Session["RCode"] = result.RCode;
                return RedirectToAction("Index", "Admin");
            }
            else if (result.IsCaptain == true && result.RCode > 0)
            {
                HttpContext.Session["Captain"] = model.Email;
                HttpContext.Session["RCode"] = result.RCode;
                return RedirectToAction("TableUser", "Admin");
            }
            return RedirectToAction("ErrorPage", "Error",new { CloseWeb = false, maintainance  = false, NotFound = true});
        }
        public ActionResult SuperAdminLogout()
        {
            try
            {
                Session.Remove("Admin");
                Session.Remove("RCode");
                Session.Remove("SuperAdmin");
                return RedirectToAction("SuperAdminLogin");
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "Error", new { CloseWeb = false, maintainance = false, NotFound = true });
            }
        }

        [Route("forgotpassword")]
        public ActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ForgotPassword(string Email, string Password)
        {
            try
            {
                var result = AccountService.Instance.ForgotPassword(Email);
                if (result.Result == true)
                {
                    var restaurant = RestaurantService.Instance.GetRestaurantByEmail(Email);
                    var encbid = HelperService.Instance.ConvertStringToHex(Email, System.Text.Encoding.Unicode);
                    string dName = HttpContext.Request.Url.GetLeftPart(UriPartial.Authority) + "/setpassword/?encmail=" + encbid;
                    string message = "<p>Click the link to set new password  for the BFoodie website :" +
                            "</br><a href=\"" + dName + "\">Click here</a></p>";
                    string subject = restaurant.Name + ": Reset Password";
                    string Head = "Reset password link";
                    //EmailTemplateModel Registeremail = new EmailTemplateModel()
                    //{
                    //    Message = message,
                    //    Destination = Email,
                    //    Subject = "BFoodie user detail"
                    //};
                    //result.Result = helperController.SendEmail(Registeremail);
                    result.Result = helperController.templateEmail(Email, subject, Head, message, restaurant.Mobile, restaurant.Address);
                    if (result.Result == false)
                    {
                        result.Messsage = "Something went wrong!";
                    }
                    result.Messsage = "we send a new password link in your register email address";
                }
                return Json(new { result = result.Result, msg = result.Messsage });
            }
            catch (Exception ex)
            {
                return Json(new { result = false, msg = "Something went wrong!" });
            }
        }

        [Route("setpassword")]
        public ActionResult setpassword(string encmail)
        {
            var BId = HelperService.Instance.ConvertHexToString(encmail, System.Text.Encoding.Unicode);
            ViewBag.MailId = BId;
            return View();
        }
        [HttpPost]
        public ActionResult setpassword(string Email, string Password,bool isAdmin)
        {
            try
            {
                var result = AccountService.Instance.ForgotPassword(Email);
                if (result.Result == true)
                {
                    var password = HelperService.Instance.Encrypt(Password);
                    AccountService.Instance.SetPassword(password, Email, isAdmin);
                }
                return Json(new { result = result.Result, msg = result.Messsage });
            }
            catch (Exception ex)
            {
                return Json(new { result = false, msg = "Something went wrong!" });
            }
        }
    }
}