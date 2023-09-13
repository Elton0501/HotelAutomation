using DataBase;
using Models;
using Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using ViewModels;

namespace HotelAutomation.Web.Controllers
{
    public class CartController : Controller
    {
        [Route("Cart")]
        public ActionResult Cart()
        {
            try
            {
                var loginUser = UserService.Instance.GetCurrentUserLogin();
                if (loginUser.isLogin == false) { return RedirectToAction("ErrorPage", "Error", new { CloseWeb = true, maintainance = false, NotFound = false }); };
                string UserIdentity = loginUser.UCode;
                int RCode = Convert.ToInt32(loginUser.RCode);
                string TCode = loginUser.TCode;
                var UserAdmin = TableService.Instance.IsUserAdmin(UserIdentity, RCode.ToString(), TCode);
                var CartOrder = new List<CartItems>();
                var restaurant = RestaurantService.Instance.GetRestaurantByID(RCode.ToString());
                var Discount = "0";
                //if (TCode == Constant.TakeAway)
                //{
                //    Discount = restaurant.TADiscount;
                //}
                //else if (TCode == Constant.HomeDelivery)
                //{
                //    Discount = restaurant.HDDiscount;
                //}
                //else
                //{
                //    Discount = restaurant.Discount;
                //}
                var Cartdata = CartService.Instance.GetUserCart(UserIdentity, RCode, TCode);
                var menudata = MenuDetailsService.Instance.GetRestMenus(RCode);
                var menuTypes =MenuDetailsService.Instance.GetMenuType();
                if (Cartdata != null)
                {
                    foreach (var item in Cartdata.CartItems)
                    {
                        var singlemenu = menudata.FirstOrDefault(x => x.MCode == item.MCode && x.RCode == RCode);
                        item.menu = singlemenu;
                        item.menuType = menuTypes.FirstOrDefault(x => x.MTCode == singlemenu.MTCode).MTDesc;
                        item.price = singlemenu.Price * item.Quantity;
                        item.Discount = Convert.ToInt32(TCode == Constant.TakeAway ? singlemenu.TADiscount != null && singlemenu.TADiscount != "" ? singlemenu.TADiscount : "0"
                            : TCode == Constant.HomeDelivery ? singlemenu.HDDiscount != null && singlemenu.HDDiscount != "" ? singlemenu.HDDiscount : "0" : singlemenu.Discount != null && singlemenu.Discount != "" ? singlemenu.Discount : "0" );
                        item.Disprice = (singlemenu.Price - singlemenu.Price * Convert.ToDecimal(item.Discount) / 100) * item.Quantity;
                        item.TotalPrice = item.Discount > 0 ? item.Disprice : item.price;
                        CartOrder.Add(item);
                    }
                }
                var DataModel = new CartViewModel();
                //DataModel.IsUserAdmin = UserAdmin.Result;
                //DataModel.UserAdmin = UserAdmin.Value1;
                DataModel.CartOrder = CartOrder;
                DataModel.Tax = restaurant.TaxApply;
                DataModel.Table = TCode;
                var TotalPRice = CartOrder.Select(x => x.TotalPrice).Sum();
                DataModel.SubTotalPrice = TotalPRice - (TotalPRice * Convert.ToInt32(Discount) / 100);
                DataModel.TotalwithoutDisc = CartOrder.Select(x=>x.price).Sum();
                DataModel.IsPlaceOrder = PlaceOrderService.Instance.IsPlaceOrder(UserIdentity, RCode, TCode);
                if (Convert.ToInt32(Discount) > 0) {
                    ViewBag.DiscountMessage = Discount + " " + "% Discount applied";
                }
                return View(DataModel);
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "Error");
            }
        }
        [HttpPost]
        public JsonResult AddInCart(int MCode)
        {
            var model = new CartDataModel();
            model.result = false;
            try
            {
                var loginUser = UserService.Instance.GetCurrentUserLogin();
                if (loginUser.isLogin == false) { model.result = false; } else {
                    int RCode = Convert.ToInt32(loginUser.RCode);
                    int TCode = Convert.ToInt32(loginUser.TCode);
                    model = CartService.Instance.AddInCart(MCode, RCode, loginUser.UCode, TCode);
                };
            }
            catch (Exception)
            {
                model.result = false;
            }
            return Json(new { Result = model.result, Count = model.Count, TotalAmount = model.TotalAmount });
        }
        public ActionResult UserCartCode()
        {
            var loginUser = UserService.Instance.GetCurrentUserLogin();
            if (loginUser.isLogin == false) {return RedirectToAction("ErrorPage", "Error", new { CloseWeb = true, maintainance = false, NotFound = false }); }
            int RCode = Convert.ToInt32(loginUser.RCode);
            var data = CartService.Instance.GetUserCartItems(loginUser.UCode, RCode, loginUser.TCode) != null ? CartService.Instance.GetUserCartItems(loginUser.UCode, RCode, loginUser.TCode).Select(x => x.MCode) : null;
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult RemoveInCart(int CartItemId, bool menu)
        {
            var model = new CartDataModel();
            model.result = false;
            int RCode = 0;
            try
            {
                var loginUser = UserService.Instance.GetCurrentUserLogin();
                model.TotalAmount = 0;
                model.TotalwithoutDisc = 0;
                if (loginUser.isLogin == false) { model.result = false; }
                else{
                    RCode = Convert.ToInt32(loginUser.RCode);
                    int TCode = Convert.ToInt32(loginUser.TCode);
                    model = CartService.Instance.RemoveInCart(CartItemId, menu, loginUser.UCode, RCode, TCode);
                }
            }
            catch (Exception)
            {
                model.result = false; 
            }
            return Json(new {Result = model.result,Count=model.Count,TotalAmount = model.TotalAmount, TotalwithoutDisc = model.TotalwithoutDisc });
        }
        [HttpPost]
        public ActionResult CartQuantity(int CartItemId, int CartQuantity)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var loginUser = UserService.Instance.GetCurrentUserLogin();
                    if (loginUser.isLogin == false) { return RedirectToAction("Login", "Account"); };
                    var ItemPrice = "0";
                    var ItemDisPrice = "0";
                    var cartItem = context.CartItems.FirstOrDefault(x => x.Id == CartItemId);
                    if (cartItem != null)
                    {
                        cartItem.Quantity = CartQuantity >= 1 ? CartQuantity : 1;
                        context.Entry(cartItem).State = EntityState.Modified;
                        context.SaveChanges();
                        ItemPrice = Convert.ToString(cartItem.Quantity * cartItem.price);
                        ItemDisPrice = Convert.ToString(cartItem.Quantity * (cartItem.price - cartItem.price * Convert.ToInt32(cartItem.Discount) / 100));
                    }
                    var cartdata = context.CartItems.Include(x=>x.Cart).Where(x => x.CartId == cartItem.CartId).ToList();
                    var restaurant = RestaurantService.Instance.GetRestaurantByID(loginUser.RCode);
                    var Discount = "0";
                    //if (loginUser.TCode == Constant.TakeAway)
                    //{
                    //    Discount = restaurant.TADiscount;
                    //}
                    //else if (loginUser.TCode == Constant.HomeDelivery)
                    //{
                    //    Discount = restaurant.HDDiscount;
                    //}
                    //else
                    //{
                    //    Discount = restaurant.Discount;
                    //}
                    var TotalAmount = cartdata.Select(x => (x.price - x.price * Convert.ToInt32(x.Discount) / 100) * x.Quantity).Sum();
                    TotalAmount = TotalAmount - (TotalAmount * Convert.ToInt32(Discount) / 100);
                    var TotalWithoutDiscount = cartdata.Select(x => x.price * x.Quantity).Sum();
                    var cartQuant = cartdata.Select(x => x.Quantity).Sum();
                    return Json(new { TotalAmount = TotalAmount.ToString(), cartQuant = cartQuant, 
                        totalWithoutDiscount = TotalWithoutDiscount, itemPrice = ItemPrice, itemDisPrice= ItemDisPrice });
                }
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "Error", new { CloseWeb = false, maintainance = false, NotFound = true });
            }
        }
        [HttpPost]
        public void DeleteCart(string UserIdentity, int RCode)
        {
            using (var context = new ApplicationDbContext())
            {
                var Cartdata = context.Cart.FirstOrDefault(x => x.CreatedBy == UserIdentity && x.RCode == RCode);
                context.Cart.Remove(Cartdata);
                context.SaveChanges();
            }
        }
        public int CartItemQuantity()
        {
            var count = 0;
            using (var context = new ApplicationDbContext())
            {
                var loginUser = UserService.Instance.GetCurrentUserLogin();
                if (loginUser.isLogin == false) { return 0; }
                int rCode = Convert.ToInt32(loginUser.RCode);
                var data = context.Cart.Include(x=>x.CartItems).FirstOrDefault(x => x.CreatedBy == loginUser.UCode && x.RCode == rCode && x.Table == loginUser.TCode);
                count = data != null && data.CartItems != null ? data.CartItems.Select(x => x.Quantity).Sum() : 0;
                return count;
            }
        }
    }
}