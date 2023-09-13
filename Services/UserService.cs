using DataBase;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ViewModels;

namespace Services
{
    public class UserService
    {
        #region singleton
        public static UserService Instance
        {
            get
            {
                if (instance == null) instance = new UserService();
                return instance;
            }
        }
        private static UserService instance { get; set; }
        public UserService()
        {
        }
        #endregion
        public RestaurantThemeViewModel GetColor(string Id)
        {
            try
            {
                var result = new RestaurantThemeViewModel();
                using (var context = new ApplicationDbContext())
                {
                    if (Id != "")
                    {
                        int ID = Convert.ToInt32(Id);
                        result = context.Restaurants.Select(x => new RestaurantThemeViewModel()
                        {
                            Id = x.Id,
                            Background = x.Background,
                            Bgblur = x.Bgblur,
                            ButtonPrimary = x.ButtonPrimary,
                            ButtonPrimaryFont = x.ButtonPrimaryFont,
                            ButtonSecondary = x.ButtonSecondary,
                            ButtonSecondaryFont = x.ButtonSecondaryFont,
                            Foodbar = x.Foodbar,
                            Heading = x.Heading,
                            Label = x.Label,
                            Navback = x.Navback,
                            Text = x.Text,
                            Logo = x.Img,
                        }).FirstOrDefault(x => x.Id == ID);
                    }
                }
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool SaveUserData(User user)
        {
            bool result = false;
            using (var context = new ApplicationDbContext())
            {
                user.CreatedOn = DateTime.Now;
                context.Entry(user).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
                result = true;
            }
            return result;
        }
        public User GetUserInfo(string UserIdentity)
        {
            var result = new User();
            if (UserIdentity != "" && UserIdentity != null)
            {
                using (var context = new ApplicationDbContext())
                {
                    result = context.User.FirstOrDefault(x => x.MobileNumber == UserIdentity || x.Email == UserIdentity);
                }
            }
            return result;
        }
        public List<User> GetUsers()
        {
            var data = new List<User>();
            using(var context = new ApplicationDbContext())
            {
                data = context.User.OrderByDescending(x=>x.Id).ToList();
                return data;
            }
        }
        public UserAuthorization GetCurrentUserLogin()
        {
            var model = new UserAuthorization();
            model.isLogin = false;
            HttpCookie WebCookie = HttpContext.Current.Request.Cookies[Constant.WebCookie];
            if (WebCookie != null) {
                var data = HelperService.Instance.Decrypt(WebCookie.Value.ToString());
                string[] CookieValue = data.Split(',');
                var uid = CookieValue[0].Replace("UId", "").Trim();
                model.UCode = uid;
                var tCode = CookieValue[1].Replace("TCode", "").Trim();
                model.TCode = tCode;
                var rCode = CookieValue[2].Replace("RCode", "").Trim();
                model.RCode = rCode;
                if (model.RCode != "" && model.RCode != null && model.TCode != null && model.TCode != ""
                        && model.UCode != "" && model.UCode != null)
                {
                    model.isLogin = true;
                }
            }
            return model;
        }

        public string checkBirthdayMessage(string mobile)
        {
            var result = "";
            using(var context= new ApplicationDbContext())
            {
                DateTime today = DateTime.Now;
                var data = context.User.FirstOrDefault(x=>x.MobileNumber == mobile || x.Email == mobile);
                if (data.DOB != null && data.DOB != "")
                {
                    DateTime date = Convert.ToDateTime(data.DOB);
                    if (today.Day == date.Day && date.Month == today.Month)
                    {
                        result = string.Format(Constant.NotificationConstant.BirthdayShortMsg,data.Name);
                    }
                }
            }
            return result;
        }
    }
}
