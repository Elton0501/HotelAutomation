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
    public class RestaurantService
    {
        #region singleton
        public static RestaurantService Instance
        {
            get
            {
                if (instance == null) instance = new RestaurantService();
                return instance;
            }
        }
        private static RestaurantService instance { get; set; }

        public RestaurantService()
        {
        }
        #endregion
        public IEnumerable<Restaurant> GetRestaurants()
        {
            using (var context = new ApplicationDbContext())
            {
                return context.Restaurants.Where(x => x.IsActive == true);
            }
        }
        public IEnumerable<Restaurant> GetRestaurantsEvents(string value)
        {
            using (var context = new ApplicationDbContext())
            {
                switch (value)
                {
                    case "YouandMe":
                        return context.Restaurants.Where(x => x.YouandMe == true).ToList();
                        break;
                    case "BirthdayParty":
                        return context.Restaurants.Where(x => x.BirthdayParty == true).ToList();
                        break;
                    case "FarewellParty":
                        return context.Restaurants.Where(x => x.FarewellParty == true).ToList();
                        break;
                    case "FamilyDinner":
                        return context.Restaurants.Where(x => x.FamilyDinner == true).ToList();
                        break;
                    default:
                        return context.Restaurants.Where(x => x.IsActive == true).ToList();
                        break;
                }
            }
        }
        public Restaurant GetRestaurantByName(string name)
        {
            using (var context = new ApplicationDbContext())
            {
                return context.Restaurants.FirstOrDefault(x => x.Name.ToLower().Replace(" ", "") == name.Replace(" ", "").ToLower() && x.IsActive == true);
            }
        }
        public Restaurant GetRestaurantByID(string rCode)
        {
            using (var context = new ApplicationDbContext())
            {
                int RCode = Convert.ToInt32(rCode);
                var data = context.Restaurants.FirstOrDefault(x => x.Id == RCode && x.IsActive == true);
                return data;
            }
        }
        public Restaurant GetRestaurantByEmail(string email)
        {
            using (var context = new ApplicationDbContext())
            {
                return context.Restaurants.FirstOrDefault(x => x.Email == email && x.IsActive == true);
            }
        }
        //public List<string> GetRestaurantLogoByID(string rCode)
        //{
        //    var result = "";
        //    using (var context = new ApplicationDbContext())
        //    {
        //        if (rCode != null && rCode != string.Empty)
        //        {
        //            int RCode = Convert.ToInt32(rCode);
        //            List<string> restDetail = new List<string>();
        //            var rest = context.Restaurants.FirstOrDefault(x => x.Id == RCode && x.IsActive == true);
        //            result = rest.Img;
        //            string discount = rest.Discount != null && rest.Discount != ""
        //            restDetail.Add(result);
        //        }
        //        return result;
        //    }
        //}
    }
}
