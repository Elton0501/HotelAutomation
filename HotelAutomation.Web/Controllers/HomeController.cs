using Microsoft.Ajax.Utilities;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using ViewModels;

namespace HotelAutomation.Web.Controllers
{
    public class HomeController : Controller
    {
        [Route("Menu")]
        public ActionResult Index(string SearchValue,int? MCCode, bool Vegstatus = true,bool NonVegstatus = true, string restType = "Food")
        {
            try
            {
                var loginUser = UserService.Instance.GetCurrentUserLogin();
                if (loginUser.isLogin == true)
                {
                    return View();
                }
                else { return RedirectToAction("ErrorPage", "Error", new { CloseWeb = true, maintainance = false, NotFound = false }); }
            } catch (Exception) { return RedirectToAction("ErrorPage", "Error", new { CloseWeb = false, maintainance = false, NotFound = true });}
        }
        [HttpPost]
        public JsonResult Menu(string SearchValue, int? MCCode,string restType,bool Vegstatus = true, bool NonVegstatus = true)
        {
            HomeViewModel searchViewMode = new HomeViewModel();
            try
            {
                var loginUser = UserService.Instance.GetCurrentUserLogin();
                if (loginUser.isLogin == true)
                {
                    int RCode = Convert.ToInt32(loginUser.RCode);
                    searchViewMode.CartModel = CartService.Instance.GetUserCartItemsHome(loginUser.UCode, RCode, loginUser.TCode);
                    var CompleteMenu = MenuDetailsService.Instance.GetRestMenus(RCode);
                    var menuType = MenuDetailsService.Instance.GetActiveMenuType(CompleteMenu);
                    var menu = MenuDetailsService.Instance.GetMenusByRCode(CompleteMenu, menuType, RCode, Vegstatus, NonVegstatus, restType, MCCode);
                    var category = MenuDetailsService.Instance.GetMenuCategoryByRCode(CompleteMenu, menuType, RCode, Vegstatus, NonVegstatus, restType);
                    searchViewMode.menuCatagories = category;
                    IEnumerable<MenuDetailsViewModel> MenuModel =
                    from m in menu
                    join c in category on m.MCCode equals c.MCCode
                    join mt in menuType on m.MTCode equals mt.MTCode
                    select new MenuDetailsViewModel
                    {
                        MCode = m.MCode,
                        MCCode = c.MCCode,
                        MDesc = m.MDesc,
                        MCDesc = c.MCDesc,
                        Amount = m.Price,
                        Image = m.img != null ? m.img : "",
                        MenuType = mt.MTDesc,
                        MComment = m.MComment,
                        isAvailCart = searchViewMode.CartModel.Count() > 0 ? searchViewMode.CartModel.FirstOrDefault(x => x.MCode == m.MCode) != null ? true : false : false, 
                        Discount = Convert.ToInt32(loginUser.TCode == Constant.TakeAway && m.TADiscount != "" ? m.TADiscount : loginUser.TCode == Constant.HomeDelivery && m.HDDiscount != "" ? m.HDDiscount : m.Discount != "" ? m.Discount : "0"),
                        DisAmount = m.Price - m.Price * Convert.ToDecimal(loginUser.TCode == Constant.TakeAway && m.TADiscount != "" ? m.TADiscount : loginUser.TCode == Constant.HomeDelivery && m.HDDiscount != "" ? m.HDDiscount : m.Discount != "" ? m.Discount : "0") / 100
                    };
                    if (SearchValue != null && SearchValue.Length > 1)
                    {
                        MenuModel = MenuModel.Where(x => x.MenuType == restType && x.MCDesc.ToLower().Contains(SearchValue.ToLower()) ||
                       x.MDesc.ToLower().Contains(SearchValue.ToLower()) ||
                       x.Amount.ToString().ToLower().Contains(SearchValue.ToLower()));
                    }
                    searchViewMode.SearchValue = SearchValue;
                    searchViewMode.MenuModel = MenuModel;
                    searchViewMode.Bar =  CompleteMenu.FirstOrDefault(x => x.MTCode == menuType.FirstOrDefault(y => y.MTDesc == Constant.Bar).MTCode) != null ? true : false ;
                    searchViewMode.Veg =  CompleteMenu.FirstOrDefault(x => x.MTCode == menuType.FirstOrDefault(y => y.MTDesc == Constant.Veg).MTCode) != null ? true : false ;
                    searchViewMode.NonVeg =  CompleteMenu.FirstOrDefault(x => x.MTCode == menuType.FirstOrDefault(y => y.MTDesc == Constant.NonVeg).MTCode) != null ? true : false;
                    searchViewMode.Beverages = CompleteMenu.FirstOrDefault(x => x.MTCode == menuType.FirstOrDefault(y => y.MTDesc == Constant.Beverage).MTCode) != null ? true : false;
                    if (MCCode != null && MCCode > 0)
                    {
                        if (MCCode != null && MCCode > 0 && MCCode != Constant.Speciality) 
                        { 
                            searchViewMode.CategoryTypeName = MenuModel.FirstOrDefault() != null ? MenuModel.FirstOrDefault().MCDesc : MCCode.HasValue ? category.FirstOrDefault(x=>x.MCCode == MCCode.Value).MCDesc : ""; 
                        }
                        //searchViewMode.Veg = restType == "Food" ? category.FirstOrDefault(x => x.MCCode == MCCode && x.Veg == true) != null ? true : false : false;
                        //searchViewMode.NonVeg = restType == "Food" ? category.FirstOrDefault(x => x.MCCode == MCCode && x.NonVeg == true) != null ? true : false : false;
                    }
                    searchViewMode.CategoryType = MCCode.HasValue ? MCCode.Value : 0;
                    var UserAdmin = TableService.Instance.IsUserAdmin(loginUser.UCode, RCode.ToString(), loginUser.TCode);
                    searchViewMode.IsUserAdmin = UserAdmin.Result;
                    searchViewMode.UserAdmin = UserAdmin.Value1;
                    searchViewMode.MenuHeading = MCCode == Constant.Speciality ? Constant.SpecialityText :
                    searchViewMode.CategoryTypeName != null && searchViewMode.CategoryTypeName != "" ? searchViewMode.CategoryTypeName :
                      restType == Constant.Food ? "Food" : restType == Constant.Bar ? "Bar" : "Beverages";
                    var serializer = new JavaScriptSerializer();
                    var data = serializer.Serialize(searchViewMode);
                    return Json(data);
                }
                else { return Json(searchViewMode); }
             
            }
            catch (Exception ex)
            {
                return Json(searchViewMode);
            }
        }
        [HttpPost]
        public ActionResult Rating(string mCode,int starRating)
        {
            try
            {
                var LoginUser = UserService.Instance.GetCurrentUserLogin();
                if (LoginUser.isLogin == true) {
                    string rcode = LoginUser.RCode;
                    int MCode = Convert.ToInt32(mCode);
                    HelperService.Instance.AddStarRating(MCode, rcode, starRating);
                }
                return Json(starRating);
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "Error", new { CloseWeb = false, maintainance = false, NotFound = true });
            }
        }
    }
}