using DataBase;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModels;

namespace Services
{
    public class AccountService
    {
        #region singleton
        public static AccountService Instance
        {
            get
            {
                if (instance == null) instance = new AccountService();
                return instance;
            }
        }
        private static AccountService instance { get; set; }

        public AccountService()
        {
        }
        #endregion
        public User saveUserDetails(string Email,string Mobile)
        {
            using(var context = new ApplicationDbContext())
            {
                var user = new User();
                if (Email != "")
                {
                    user = context.User.FirstOrDefault(x=>x.Email == Email);
                }
                else
                {
                    user = context.User.FirstOrDefault(x => x.MobileNumber == Mobile);
                }
                if (user == null)
                {
                    var data = new User();
                    if (Email != null && Email != "")
                    {
                        data.Email = Email;
                    }
                    if(Mobile != null && Mobile != "")
                    {
                        data.MobileNumber = Mobile;
                    }
                    data.IsActive = true;
                    data.CreatedOn = DateTime.Now;
                    data.RoleId = context.Roles.FirstOrDefault(x=>x.RoleName == "User").RoleId;
                    context.User.Add(data);
                    context.SaveChanges();
                    return data;
                }
                return user;
            }
        }
        public OTPModel GenerateOTP(int id)
        {
            OTPModel model = new OTPModel();
            model.Result = false;
            int randomNumber = 134679; 
            Random numberGen = new Random();
            int minimumRange = 111111;
            int maximumRange = 999999;
            randomNumber = numberGen.Next(minimumRange, maximumRange);
            using (var context = new ApplicationDbContext())
            {
                model.Otp = randomNumber.ToString();
                EmailOTP oTP = new EmailOTP();
                oTP.OTP = randomNumber;
                oTP.CreatedOn = DateTime.Now;
                oTP.UId = id;
                oTP.UserDetails = Guid.NewGuid();
                context.EmailOTP.Add(oTP);
                context.SaveChanges();
                model.Details = oTP.UserDetails.ToString();
                model.Result = true;
            }
            return model;
        }
        public OTPModel OtpVerify(OTPModel verifyDetail)
        {
            var data = new OTPModel();
            data.Result = false;
            data.Status = "failed";
            using(var context = new ApplicationDbContext())
            {
                var EmailOTP = context.EmailOTP.FirstOrDefault(x=>x.UserDetails.ToString() == verifyDetail.Details &&
                verifyDetail.UserIdentity == verifyDetail.UserIdentity && x.OTP.ToString() == verifyDetail.Otp);
                if(EmailOTP != null)
                {
                    data.Result = true;
                    data.Status = "Success";
                    data.UserIdentity = verifyDetail.UserIdentity;
                    context.EmailOTP.Remove(EmailOTP);
                    context.SaveChanges();
                }
            }
            return data;
        }
        public ResultModel ForgotPassword(string email)
        {
            var result = new ResultModel();
            result.Result = false;
            using (var context = new ApplicationDbContext())
            {
                var data = context.Restaurants.FirstOrDefault(x => x.Email.ToLower() == email.ToLower());
                if (data == null)
                {
                    result.Messsage = "This email id is not exist";
                }
                else
                {
                    result.Result = true;
                }
            }
            return result;
        }
        public ResultModel SetPassword(string password, string email, bool isAdmin)
        {
            var result = new ResultModel();
            result.Result = false;
            using (var context = new ApplicationDbContext())
            {
                var data = context.Restaurants.FirstOrDefault(x => x.Email.ToLower() == email.ToLower());
                if (data != null)
                {
                    if (!isAdmin)
                    {
                        data.CaptainKey = password;
                    }
                    else
                    {
                        data.Key = password;
                    }
                    context.Entry(data).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                    result.Result = true;
                }
                else
                {
                    result.Messsage = "The user is not exist";
                }
            }
            return result;
        }
    }
}
