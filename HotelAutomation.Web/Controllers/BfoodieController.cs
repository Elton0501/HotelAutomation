using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ViewModels;

namespace HotelAutomation.Web.Controllers
{
    public class BfoodieController : Controller
    {
        [Route("Bfoodie")]
        public ActionResult Home()
        {
            var model = new BFoodieViewModel();
            model.Restaurants = RestaurantService.Instance.GetRestaurants();
            return View(model);
        }
        public ActionResult BfoodieEvents() 
        {
            try
            {
                return View();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public ActionResult RestViewForUserChoice(string selectView)
        {
            try
            {
                var data = RestaurantService.Instance.GetRestaurantsEvents(selectView);
                ViewBag.EventName = selectView;
                return View(data);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}