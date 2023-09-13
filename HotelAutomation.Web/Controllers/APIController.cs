using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Web.Http;
using ViewModels;

namespace HotelAutomation.Web.Controllers
{
    public class APIController : ApiController
    {
        [HttpGet]
        public OTPModel OtpGenrate(string PhoneNumber)
        {
            OTPModel model = new OTPModel();
            try
            {
                var api_key = ConfigurationManager.AppSettings["OtpApiKey"];
                var otpgenerate = string.Format("https://2factor.in/API/V1/{0}/SMS/{1}/AUTOGEN", api_key, PhoneNumber);
                using (var client = new HttpClient())
                {
                    //client.BaseAddress = new Uri(otpgenerate);
                    var responseTask = client.GetAsync(otpgenerate);
                    responseTask.Wait();
                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        var readTask = result.Content.ReadAsAsync<OTPModel>();
                        readTask.Wait();
                        model = readTask.Result;
                        model.Result = true;
                    }
                    else //web api sent error response 
                    {
                        model.Status = "failed";
                        model.Result = false;
                        //log response status here..
                        ModelState.AddModelError(string.Empty, "Server error. Sorrry somrthing went wrong.");
                    }
                }

            }
            catch (Exception)
            {
                model.Status = "failed";
                model.Result = false;
            }
            return model;
        }
        [HttpGet]
        public OTPModel OtpVerify(OTPModel details)
        {
            OTPModel model = new OTPModel();
            model.UserIdentity = details.UserIdentity;
            model.Details = details.Details;
            try
            {
                var api_key = ConfigurationManager.AppSettings["OtpApiKey"];
                var otpgenerate = string.Format("https://2factor.in/API/V1/{0}/SMS/VERIFY/{1}/{2}", api_key, details.Details, details.Otp);
                using (var client = new HttpClient())
                {
                    //client.BaseAddress = new Uri(otpgenerate);
                    var responseTask = client.GetAsync(otpgenerate);
                    responseTask.Wait();
                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        var readTask = result.Content.ReadAsAsync<OTPModel>();
                        readTask.Wait();
                        model = readTask.Result;
                    }
                    else //web api sent error response 
                    {
                        model.Status = "failed";
                        //log response status here..
                        ModelState.AddModelError(string.Empty, "Server error. Sorrry somrthing went wrong.");
                    }
                }

            }
            catch (Exception)
            {
                model.Status = "failed";
            }
            return model;
        }
    }
}
