using DataBase;
using Microsoft.Ajax.Utilities;
using Models;
using Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using ViewModels;

namespace HotelAutomation.Web.Controllers
{
    [AdminAuthorizationFilterAttribute]
    public class AdminController : Controller
    {
        CheckoutController Checkout = new CheckoutController();
        #region Dashboard
        public ActionResult Index()
        {
            AdminIndexViewModel model = new AdminIndexViewModel();
            var rCode = HttpContext.Session["RCode"];
            if (rCode != null) {
                int RCode = Convert.ToInt32(rCode);
                using(var context = new ApplicationDbContext())
                {
                    DateTime Currentdate = DateTime.Now;
                    var user = context.User.ToList();
                    var rest = context.Restaurants.FirstOrDefault(x=>x.Id == RCode);
                    var RestOrders = context.Orders.Include(x => x.OrderItem).Where(x => x.BillPayed == true && x.RCode == RCode).ToList();
                    model.User = RestOrders.Select(x => new UserCount()
                    {
                        User = user.FirstOrDefault(b=>b.MobileNumber == x.CreatedBy || b.Email == x.CreatedBy) != null? user.FirstOrDefault(b => b.MobileNumber == x.CreatedBy || b.Email == x.CreatedBy).Name : x.CreatedBy,
                        Count = RestOrders.Where(z=>z.CreatedBy == x.CreatedBy).Count()
                    }).DistinctBy(a=>a.User).ToList();
                    model.Orders = RestOrders;
                    model.YOrdersCount = RestOrders.Where(x => x.CreatedOn.Year == Currentdate.Year).ToList().Count;
                    model.YearlyIncome = RestOrders.Where(x => x.CreatedOn.Year == Currentdate.Year).ToList().Select(x=>x.TotalAmount).Sum();
                    model.MOrdersCount = RestOrders.Where(x => x.CreatedOn.Year == Currentdate.Year && x.CreatedOn.Month == Currentdate.Month).ToList().Count;
                    model.MonthlyIncome = RestOrders.Where(x => x.CreatedOn.Year == Currentdate.Year && x.CreatedOn.Month == Currentdate.Month).ToList().Select(x=>x.TotalAmount).Sum();
                    model.DineIn = RestOrders.Where(x => x.Table != Constant.HomeDelivery && x.Table != Constant.TakeAway).Count() > 0 ? RestOrders.Where(x => x.Table != Constant.HomeDelivery && x.Table != Constant.TakeAway).Count() * 100 / RestOrders.Count : 0;
                    model.TakeAway = RestOrders.Where(x => x.Table == Constant.TakeAway).Count() > 0 ? RestOrders.Where(x => x.Table == Constant.TakeAway).Count() * 100 / RestOrders.Count: 0 ;
                    model.Parcel = RestOrders.Where(x => x.Table == Constant.HomeDelivery).Count() > 0 ? RestOrders.Where(x => x.Table == Constant.HomeDelivery).Count() * 100 / RestOrders.Count : 0;
                    model.OwnerName = rest.OwnerName != "" && rest.OwnerName != null ? rest.OwnerName : rest.Email;
                    model.BillDiscounts = BillDiscountService.Instance.GetListBillDiscount(rCode.ToString());
                }
            }
            else
            {
                return RedirectToAction("SuperAdminLogin", "AdminAccount");
            }
            return View(model);
        }
        [HttpPost]
        public string RestaurantDiscount(string Discount, string TADiscount, string HDDiscount)
        {
            var result = "false";
            try
            {
                var rCode = HttpContext.Session["RCode"];
                int RCode = Convert.ToInt32(rCode);
                using (var context = new ApplicationDbContext())
                {
                    var data = context.Restaurants.FirstOrDefault(x => x.Id == RCode);
                    if (data != null)
                    {
                        //data.Discount = Discount;
                        //data.TADiscount = TADiscount;
                        //data.HDDiscount = HDDiscount;
                        context.Entry(data).State = EntityState.Modified;
                        context.SaveChanges();
                        result = "true";
                    }
                }
            }
            catch (Exception ex)
            {
                result = "false";
            }
            return result;
        }
        #endregion
        #region CurrentStatus
        public ActionResult CurrentStatus(string search,string FilterType)
        {

            AdminIndexViewModel model = new AdminIndexViewModel();
            var rCode = HttpContext.Session["RCode"];
            ViewBag.RCode = rCode.ToString();
            //if (rCode != null)
            //{
            //    int RCode = Convert.ToInt32(rCode);
            //    ViewBag.RCode = rCode.ToString();
            //    using (var context = new ApplicationDbContext())
            //    {
            //        DateTime Currentdate = DateTime.Now;
            //        decimal tax = 0;
            //        var restaurant = context.Restaurants.FirstOrDefault(x => x.Id == RCode);
            //        tax = restaurant.Tax;
            //        var menu = context.Menu.Where(x => x.RCode == RCode).ToList();
            //        var menuCat = context.MenuCategory.Where(x => x.RCode == RCode).ToList();
            //        var placeOrder = context.PlaceOrderItems.Include(x => x.PlaceOrder).Where(x => x.PlaceOrder.RCode == RCode).ToList();
            //        var PlaceOrderItemsdata = placeOrder.ToList()
            //        .Select(i =>
            //        { 
            //            i.menu = menu.FirstOrDefault(x => x.MCode == i.MCode);
            //            i.menuType = menuCat.FirstOrDefault(x => x.MCCode == i.menu.MCCode).MCDesc;
            //            i.price = i.price * i.Quantity - ((i.price * i.Quantity) * Convert.ToInt32(i.Discount) / 100);
            //            return i;
            //        }).ToList();
            //        var PlaceOrderdata = PlaceOrderItemsdata.DistinctBy(x => x.PlaceOrder.Id).Select(x=>x.PlaceOrder);
            //        var tableData = PlaceOrderItemsdata.DistinctBy(x => x.PlaceOrder.Id);
            //        model.AdminCartViewModel = tableData.Select(x => new AdminCartViewModel()
            //        {
            //            POId = x.PlaceOrder.Id,
            //            Table = x.PlaceOrder.Table,
            //            PlaceOrderItems = PlaceOrderItemsdata.Where(y => y.PlaceOrder.Id == x.PlaceOrder.Id).ToList(),
            //            TotalPrice = PlaceOrderItemsdata.Where(y => y.PlaceOrder.Id == x.PlaceOrder.Id).Select(z => z.price).Sum(),
            //            User = x.PlaceOrder.CreatedBy,
            //            Address = PlaceOrderItemsdata.FirstOrDefault(y => y.PlaceOrder.Table == x.PlaceOrder.Table && y.PlaceOrder.Address != null && y.PlaceOrder.Address != "") != null ? PlaceOrderItemsdata.FirstOrDefault(y => y.PlaceOrder.Table == x.PlaceOrder.Table && y.PlaceOrder.Address != null && y.PlaceOrder.Address != "").PlaceOrder.Address : "",
            //            OrderAt = x.PlaceOrder.CreatedOn,
            //            RCode = RCode.ToString(),
            //        }).OrderByDescending(x => x.OrderAt).ToList();
            //        if (search != null && search != "")
            //        {
            //            if (FilterType == "1")
            //            {
            //                model.AdminCartViewModel = model.AdminCartViewModel.Where(x => x.Table == search).ToList();
            //                ViewBag.SearchValue = search;
            //            }
            //            else if(FilterType == "2")
            //            {
            //                model.AdminCartViewModel = model.AdminCartViewModel.Where(x => x.User.ToLower().Contains(search.ToLower())).ToList();
            //                ViewBag.SearchValue = search;
            //            }
            //        }
            //    }
            //}
            if (rCode == null)
            {
                return RedirectToAction("SuperAdminLogin", "AdminAccount");
            }
            return View();
        }
        public ActionResult CurrentStatusData(string search, string FilterType)
        {
            AdminIndexViewModel model = new AdminIndexViewModel();
            var rCode = HttpContext.Session["RCode"];
            int RCode = Convert.ToInt32(rCode);
            ViewBag.RCode = rCode.ToString();
            using (var context = new ApplicationDbContext())
            {
                DateTime Currentdate = DateTime.Now;
                decimal tax = 0;
                var restaurant = context.Restaurants.FirstOrDefault(x => x.Id == RCode);
                tax = restaurant.Tax;
                var menu = context.Menu.Where(x => x.RCode == RCode).ToList();
                var menuCat = context.MenuCategory.Where(x => x.RCode == RCode).ToList();
                var placeOrder = context.PlaceOrderItems.Include(x => x.PlaceOrder).Where(x => x.PlaceOrder.RCode == RCode).ToList();
                var PlaceOrderItemsdata = placeOrder.ToList()
                .Select(i =>
                {
                    i.menu = menu.FirstOrDefault(x => x.MCode == i.MCode);
                    i.menuType = menuCat.FirstOrDefault(x => x.MCCode == i.menu.MCCode).MCDesc;
                    i.price = i.price * i.Quantity - ((i.price * i.Quantity) * Convert.ToInt32(i.Discount) / 100);
                    i.PlaceOrder.PlaceOrderItems = null;
                    return i;
                }).ToList();
                var PlaceOrderdata = PlaceOrderItemsdata.DistinctBy(x => x.PlaceOrder.Id).Select(x => x.PlaceOrder);
                var tableData = PlaceOrderItemsdata.DistinctBy(x => x.PlaceOrder.Id);
                model.AdminCartViewModel = tableData.Select(x => new AdminCartViewModel()
                {
                    POId = x.PlaceOrder.Id,
                    Table = x.PlaceOrder.Table,
                    PlaceOrderItems = PlaceOrderItemsdata.Where(y => y.PlaceOrder.Id == x.PlaceOrder.Id).ToList(),
                    TotalPrice = PlaceOrderItemsdata.Where(y => y.PlaceOrder.Id == x.PlaceOrder.Id).Select(z => z.price).Sum(),
                    User = x.PlaceOrder.CreatedBy,
                    Address = PlaceOrderItemsdata.FirstOrDefault(y => y.PlaceOrder.Table == x.PlaceOrder.Table && y.PlaceOrder.Address != null && y.PlaceOrder.Address != "") != null ? PlaceOrderItemsdata.FirstOrDefault(y => y.PlaceOrder.Table == x.PlaceOrder.Table && y.PlaceOrder.Address != null && y.PlaceOrder.Address != "").PlaceOrder.Address : "",
                    OrderAt = x.PlaceOrder.CreatedOn,
                    RCode = RCode.ToString(),
                }).OrderByDescending(x => x.OrderAt).ToList();
                if (search != null && search != "")
                {
                    if (FilterType == "1")
                    {
                        model.AdminCartViewModel = model.AdminCartViewModel.Where(x => x.Table == search).ToList();
                        ViewBag.SearchValue = search;
                    }
                    else if (FilterType == "2")
                    {
                        model.AdminCartViewModel = model.AdminCartViewModel.Where(x => x.User.ToLower().Contains(search.ToLower())).ToList();
                        ViewBag.SearchValue = search;
                    }
                }
            }
            return PartialView(model);
        }
        public JsonResult DiscountPerItem(string POId, string Discount, string PunchedBy)
        {
            try
            {
                var result =new DiscountPerItemViewModel();
                result.result = "false";
                using(var context = new ApplicationDbContext())
                {
                    var punchedBy = context.CaptainDetails.FirstOrDefault(x => x.UniqueCode == PunchedBy).Name;
                    long poid = Convert.ToInt64(POId);
                    var data = context.PlaceOrderItems.Include(x=>x.PlaceOrder).FirstOrDefault(x=>x.Id == poid);
                    data.Discount =  Discount;
                    data.PunchedBy = MenuDetailsService.Instance.CheckPunchedBy(data.PlaceOrder.RCode, data.MCode, Discount, punchedBy,data.PlaceOrder.Table);
                    context.Entry(data).State = EntityState.Modified;
                    context.SaveChanges();
                    result.result = "true";
                    result.Amount =(data.price * data.Quantity) - (data.price * data.Quantity * Convert.ToDecimal(data.Discount) / 100);
                    result.Discount = Discount;
                }
                return Json(result,JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost]
        public string SaveOrEditUserAddress(string POId,string Address)
        {
            try
            {
                using(var context = new ApplicationDbContext())
                {
                    string result = "false";
                    if (Address != "" && Address != null && POId != "" && POId != null)
                    {
                        long poid = Convert.ToInt64(POId);
                        var data = context.PlaceOrders.FirstOrDefault(x => x.Id == poid);
                        data.Address = Address;
                        context.Entry(data).State = EntityState.Modified;
                        context.SaveChanges();
                        result = "true";
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        #endregion
        #region Orders
        public ActionResult Orders()
        {
            var data = new List<Orders>();
            var rCode = HttpContext.Session["RCode"]; int RCode = Convert.ToInt32(rCode);
            if (rCode != null)
            {
                using (var context= new ApplicationDbContext())
                {
                    ViewBag.TableCount = Convert.ToInt32(RestaurantService.Instance.GetRestaurantByID(rCode.ToString()).TableCount);
                    ViewBag.Tax = context.Restaurants.FirstOrDefault(x=>x.Id == RCode).Tax;
                    ViewBag.RCode = rCode.ToString();
                    ViewBag.OrdersCount = context.Orders.OrderByDescending(x => x.CreatedOn).Include(x => x.OrderItem).Where(x => x.BillPayed == true && x.RCode == RCode).ToList().Count();
                  //  data = context.Orders.OrderByDescending(x=>x.CreatedOn).Include(x => x.OrderItem).Where(x => x.BillPayed == true && x.RCode == RCode).ToList();
                }
            }
            else
            {
                return RedirectToAction("SuperAdminLogin", "AdminAccount");
            }
            return View();
        }
        public ActionResult _OrderTable(string FilterByDay, string FilterPaymentType, string FilterTableNo,string FilterByDate)
        {
            var data = new List<Orders>();
            var rCode = HttpContext.Session["RCode"]; int RCode = Convert.ToInt32(rCode);
            if (rCode != null)
            {
                using (var context = new ApplicationDbContext())
                {
                    ViewBag.RCode = rCode.ToString();
                    ViewBag.TableCount = Convert.ToInt32(RestaurantService.Instance.GetRestaurantByID(rCode.ToString()).TableCount);
                    ViewBag.Tax = context.Restaurants.FirstOrDefault(x => x.Id == RCode).Tax;
                    data = context.Orders.OrderByDescending(x => x.CreatedOn).Include(x => x.OrderItem).Where(x => x.BillPayed == true && x.RCode == RCode).ToList();
                    ViewBag.OrdersCount = data.Count();
                    if (!string.IsNullOrEmpty(FilterByDay))
                    {
                        if (FilterByDay == "1")
                        {
                            data = data.Where(x => x.CreatedOn.Date == DateTime.Now.Date).ToList();
                        }
                        else if (FilterByDay == "2")
                        {
                            DateTime week = DateTime.Now.AddDays(-7);
                            data = data.Where(x => x.CreatedOn >= week).ToList();
                        }
                        else if (FilterByDay == "3")
                        {
                            data = data.Where(x => x.CreatedOn.Month == DateTime.Now.Month).ToList();
                        }
                        else
                        {
                            data = context.Orders.OrderByDescending(x => x.CreatedOn).Include(x => x.OrderItem).Where(x => x.BillPayed == true && x.RCode == RCode).ToList();
                        }
                    }

                    if (!string.IsNullOrEmpty(FilterPaymentType) && FilterPaymentType != "0")
                    {
                        data = data.Where(x => x.PaymentType == Convert.ToInt32(FilterPaymentType)).ToList();
                    }
                    if (!string.IsNullOrEmpty(FilterTableNo) && FilterTableNo != "0")
                    {
                        data = data.Where(x => x.Table == FilterTableNo).ToList();
                    }
                    if (!string.IsNullOrEmpty(FilterByDate))
                    {
                        data = data.Where(x => x.CreatedOn.Date == Convert.ToDateTime(FilterByDate)).ToList();
                    }
                }
            }
            else
            {
                return RedirectToAction("SuperAdminLogin", "AdminAccount");
            }
            return PartialView(data);
        }
        public ActionResult OrderDetails(string OrderId)
        {
            var result = new List<OrdersViewModel>();
            try
            {
                using(var context = new ApplicationDbContext())
                {
                    var data = context.Orders.Include(x => x.OrderItem).FirstOrDefault(x => x.OrderId == OrderId);
                    var menu = context.Menu.Where(x=>x.RCode == data.RCode).ToList();
                    var menuCat = context.MenuCategory.Where(x=>x.RCode == data.RCode).ToList();
                    result = data.OrderItem.Select(x => new OrdersViewModel
                    {
                        Id = x.Id,
                        OrderId = x.OrderId,
                        Menu = menu.FirstOrDefault(y => y.MCode == x.MCode).MDesc,
                        MenuCategory = menuCat.FirstOrDefault(y => y.MCCode == menu.FirstOrDefault(z => z.MCode == x.MCode).MCCode).MCDesc,
                        Price = x.Price,
                        Quantity = x.Quantity,
                        OrderDate = data.CreatedOn,
                        UserIdentity = data.CreatedBy,
                        Discount = x.Discount,
                        PunchedBy = x.PunchedBy,
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage","Error");
            }
            return PartialView(result);
        }
        #endregion
        #region MenuRating
        public ActionResult MenuRating()
        {
            var result =new List<MenuRating>();
            try
            {
                var rCode = HttpContext.Session["RCode"];
                if (rCode != null)
                {
                    int RCode = Convert.ToInt32(rCode);
                    using (var context = new ApplicationDbContext())
                    {
                        var menu = context.Menu.Where(x => x.RCode == RCode && x.Rating != null && x.Rating != "0").ToList();
                        result = menu.Select(x => new MenuRating()
                        {
                            Menu = x.MDesc,
                            Rating = x.Rating,
                            Percentage = Convert.ToInt32(x.Rating) * 100 / 5,
                        }).ToList();
                    }
                }
                else
                {
                    return RedirectToAction("SuperAdminLogin", "AdminAccount");
                }
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "Error");
            }
            return View(result);
        }
        #endregion
        #region Charts
        public ActionResult pieChart()
        {
            var result = new List<WebCharts>();
            try
            {
                var rCode = HttpContext.Session["RCode"]; int RCode = Convert.ToInt32(rCode);
                if (rCode != null)
                {
                    using (var context = new ApplicationDbContext())
                    {
                        var data = context.Orders.OrderByDescending(x => x.CreatedOn).Where(x => x.BillPayed == true && x.RCode == RCode).ToList();
                        result = data.Select(x => new WebCharts() { 
                            KeyOrder = (int)x.CreatedOn.DayOfWeek,
                            Key= x.CreatedOn.DayOfWeek.ToString(),
                            Value = data.Where(y=>y.CreatedOn.DayOfWeek == x.CreatedOn.DayOfWeek).Count() * 100 / data.Count
                        }).DistinctBy(x=>x.Key).OrderBy(x=>x.KeyOrder).ToList();
                    }
                }
                else
                {
                    return RedirectToAction("SuperAdminLogin", "AdminAccount");
                }
            }
            catch (Exception)
            {
                throw;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult doughnut()
        {
            var result = new List<WebCharts>();
            try
            {
                var rCode = HttpContext.Session["RCode"]; int RCode = Convert.ToInt32(rCode);
                if (rCode != null)
                {
                    using (var context = new ApplicationDbContext())
                    {
                        var data = context.Orders.OrderByDescending(x => x.CreatedOn).Where(x => x.BillPayed == true && x.RCode == RCode).ToList();
                        result = data.Select(x => new WebCharts() { 
                            Key= x.CreatedOn.ToString("MMMM"),
                            Value = data.Where(y=>y.CreatedOn.Month == x.CreatedOn.Month).Count()
                        }).DistinctBy(x=>x.Key).ToList();
                    }
                }
                else
                {
                    return RedirectToAction("SuperAdminLogin", "AdminAccount");
                }
            }
            catch (Exception)
            {
                throw;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult lineChart()
        {
            var result = new List<WebCharts>();
            try
            {
                var rCode = HttpContext.Session["RCode"]; int RCode = Convert.ToInt32(rCode);
                if (rCode != null)
                {
                    using (var context = new ApplicationDbContext())
                    {
                        var OrderItemList = new List<OrderitemViewModel>();
                        var FilterItemList = new List<OrderitemViewModel>();
                        var menuType = context.MenuTypes.FirstOrDefault(x=>x.MTDesc == Constant.Bar);
                        var restMenu = context.Menu.ToList();
                        var rest = context.Restaurants.ToList();
                        var myrest = rest.FirstOrDefault(x => x.Id == RCode);
                        var nearbyrest = rest.Where(x => x.City == myrest.City);
                        var AllOrderItems = context.OrderItem.Include(x => x.Orders).OrderByDescending(x => x.CreatedOn).Where(x=>x.CreatedOn.Month == DateTime.Now.Month
                            && x.CreatedOn.Year == DateTime.Now.Year).ToList();
                        OrderItemList = AllOrderItems.Select(x => new OrderitemViewModel
                        {
                            MDesc = restMenu.FirstOrDefault(a=>a.MCode == x.MCode).MDesc,
                            MType = restMenu.FirstOrDefault(a => a.MCode == x.MCode).MTCode,
                            MCode = x.MCode,
                            RCode = x.Orders.RCode,
                        }).ToList();
                        OrderItemList = OrderItemList.Where(x => x.MType != menuType.MTCode).ToList();
                        FilterItemList = (from Od in OrderItemList
                                          where nearbyrest.Any(x=>x.Id == Od.RCode) select Od).ToList();
                        result = FilterItemList.Select(x => new WebCharts()
                        {
                            Key = x.MDesc,
                            Value = FilterItemList.Where(z=>z.MCode == x.MCode).Count()
                        }).DistinctBy(x => x.Key).OrderByDescending(x=>x.Value).ToList();
                        result = result.Count() > 8 ? result.Take(8).ToList() : result;
                    }
                }
                else
                {
                    return RedirectToAction("SuperAdminLogin", "AdminAccount");
                }
            }
            catch (Exception)
            {
                throw;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult barChart(string First, string Second)
        {
            DateTime first = First != null && First != string.Empty ? Convert.ToDateTime(First) : DateTime.Now.AddDays(-7);
            DateTime second = Second != null && Second != string.Empty ? Convert.ToDateTime(Second).AddDays(1) : DateTime.Now;
            var result = new List<WebCharts>();
            try
            {
                var rCode = HttpContext.Session["RCode"]; int RCode = Convert.ToInt32(rCode);
                if (rCode != null)
                {
                    using (var context = new ApplicationDbContext())
                    {
                        var data = context.Orders.OrderByDescending(x => x.CreatedOn).Where(x => x.BillPayed == true 
                        && x.RCode == RCode && x.CreatedOn < second && x.CreatedOn >= first).ToList();
                        result = data.Select(x => new WebCharts() { 
                            Key= x.CreatedOn.Date.ToString("MMM-dd"),
                            Value = data.Where(y => y.CreatedOn.Date == x.CreatedOn.Date).Count()
                        }).DistinctBy(x=>x.Key).ToList();
                    }
                }
                else
                {
                    return RedirectToAction("SuperAdminLogin", "AdminAccount");
                }
            }
            catch (Exception)
            {
                throw;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion`
        #region RestDetails
        public ActionResult RestDetails()
        {
            var data = new Restaurant();
            var rCode = HttpContext.Session["RCode"];
            if (rCode != null)
            {
                data = RestaurantService.Instance.GetRestaurantByID(rCode.ToString());
            }
            return View(data);
        }
        #endregion
        #region Remove
        public ActionResult RemoveOrderItem(int Id)
        {
            bool result = false;
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var data = context.OrderItem.FirstOrDefault(x => x.Id == Id);
                    string OId = data.OrderId;
                    if (data != null)
                    {
                        context.OrderItem.Remove(data);
                        context.SaveChanges();
                        result = true;

                        var order = context.Orders.Include(x=>x.OrderItem).FirstOrDefault(x=>x.OrderId == OId);
                        if (order != null)
                        {
                            int rCode = order.RCode;
                            var rest = context.Restaurants.FirstOrDefault(x => x.Id == rCode);
                            order.TotalAmount = order.OrderItem.Select(x => x.Price * x.Quantity).Sum();
                            if (rest.TaxApply == false)
                            {
                                order.TaxAmount = order.TotalAmount * rest.Tax / 100;
                                order.TotalAmount = order.TotalAmount + order.TaxAmount;
                            }
                            else
                            {
                                order.TaxAmount = order.TotalAmount * rest.Tax / 100;
                            }
                            context.Entry(order).State = EntityState.Modified;
                            context.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception)
            {
                 result = false;
            }
            return Json(result);
        }
        public ActionResult RemoveFromTable(int Id)
        {
            bool result = false;
            try
            {
                result = TableService.Instance.RemoveFromTable(Id);
            }
            catch (Exception)
            {
                return RedirectToAction("SuperAdminLogin", "AdminAccount");
            }
            return Json(result);
        }
        public ActionResult RemoveOrderByAdmin(string UCode, string TCode, long Id)
        {
            bool result = false;
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var rCode = HttpContext.Session["RCode"];
                    if (rCode != null && UCode != string.Empty && UCode != null && TCode != null && TCode != string.Empty)
                    {
                        int RCode = Convert.ToInt32(rCode.ToString());
                        var data = context.PlaceOrders.Where(x => x.Id == Id && x.CreatedBy == UCode && x.RCode == RCode && x.Table == TCode).ToList();
                        if (data != null && data.Count > 0)
                        {
                            context.PlaceOrders.RemoveRange(data);
                            context.SaveChanges();
                        }
                        result = true;
                    }
                }
            }
            catch (Exception)
            {
                result = false;
            }
            return Json(result);
        }
        [HttpPost]
        public ActionResult RemovePlaceOrderItem(long PoIId)
        {
            bool result = false;
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var rCode = HttpContext.Session["RCode"];

                    if (rCode != null)
                    {
                        int RCode = Convert.ToInt32(rCode.ToString());
                        var data = context.PlaceOrderItems.Where(x => x.Id == PoIId).ToList();
                        if (data != null && data.Count > 0)
                        {
                            context.PlaceOrderItems.RemoveRange(data);
                            context.SaveChanges();
                        }
                        result = true;
                    }
                }
            }
            catch (Exception)
            {
                result = false;
            }
            return Json(result);
        }
        #endregion
        #region TableUser
        public ActionResult TableUser()
        {
            var TableStatus = new RestTable();
            //Uri link = new Uri(HttpContext.Request.Url.AbsoluteUri);
            //ViewBag.TableURL = link.AbsoluteUri.Substring(0, link.AbsoluteUri.LastIndexOf("/Admin")) + "/Account/Login/?TCode=";
            var rCode = HttpContext.Session["RCode"];
            var AlldataTable = new List<Table>();
            int TableCount = Convert.ToInt32(RestaurantService.Instance.GetRestaurantByID(rCode.ToString()).TableCount);
            for (int i = 1; i < TableCount + 1; i++)
            {
                var SingleTable = new Table();
                SingleTable = new Table()
                {
                    TCode = i.ToString(),
                    RCode = rCode.ToString(),
                };
                AlldataTable.Add(SingleTable);
            }
            //For TakeAway Table add 
            var table = new List<Table>() {
                new Table {TCode = Constant.TakeAway, RCode = rCode.ToString()},
                new Table {TCode = Constant.HomeDelivery, RCode = rCode.ToString()},
            };
            AlldataTable.AddRange(table);
            //End
            var Tabledata = TableService.Instance.TableUser(rCode.ToString());
            var data = AlldataTable.Select(x => new Table()
            {
                Id = Tabledata != null && Tabledata.Count() > 0 && Tabledata.Where(y => y.TCode == x.TCode).FirstOrDefault() != null ? Tabledata.Where(y => y.TCode == x.TCode).FirstOrDefault().Id : 0,
                TCode = x.TCode,
                RCode = x.RCode,
                CreatedBy = Tabledata != null && Tabledata.Count() > 0 && Tabledata.Where(y => y.TCode == x.TCode).FirstOrDefault() != null ? Tabledata.Where(y=>y.TCode == x.TCode).FirstOrDefault().CreatedBy : "",
                CreatedOn = Tabledata != null && Tabledata.Count() > 0 && Tabledata.Where(y => y.TCode == x.TCode).FirstOrDefault() != null ? Tabledata.Where(y => y.TCode == x.TCode).FirstOrDefault().CreatedOn : DateTime.Now,
            });
            TableStatus.Tables = data;
            TableStatus.UserTables = Tabledata;
            var actualTable = data.Take(data.Count() - 2);
            var tableNo = actualTable.Where(x => x.CreatedBy.Length > 0).Count();
            var Takeaway = actualTable.Where(x => x.TCode == Constant.TakeAway && x.CreatedBy.Length > 0).Count();
            var HomeDel = actualTable.Where(x => x.TCode == Constant.HomeDelivery && x.CreatedBy.Length > 0).Count();
            int otherTable = Takeaway + HomeDel;
            TableStatus.TableRese = Convert.ToString(tableNo - otherTable);
            var tableAvailNo = actualTable.Where(x => x.CreatedBy == null || x.CreatedBy == string.Empty).Count();
            TableStatus.TableAvail = Convert.ToString(tableAvailNo - otherTable);
            return View(TableStatus);
        }
        public ActionResult TableOrdersPartial(string TCode, string UCode, bool isSingleuser)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    decimal tax = 0;
                    var Model = new TableOrderModel();
                    var rCode = HttpContext.Session["RCode"];
                    int RCode = Convert.ToInt32(rCode);
                    var restaurant = context.Restaurants.FirstOrDefault(x => x.Id == RCode);
                    tax = restaurant.Tax;
                    var PData = context.PlaceOrders.Include(x => x.PlaceOrderItems).Where(x => x.Table == TCode && x.RCode == RCode).ToList();
                    if (UCode != null && UCode != string.Empty)
                    {
                        PData = PData.Where(x => x.CreatedBy == UCode).ToList();
                    }
                    var PlaceOrderItem = new List<PlaceOrderItems>();
                    //Model.PlaceOrders = PData.Select(x => {
                    //    x.TotalPrice = x.PlaceOrderItems.Select(y => y.price * y.Quantity - ((y.price * y.Quantity) * Convert.ToInt32(y.Discount) / 100)).Sum();
                    //    return x;
                    //}).ToList();
                    Model.PlaceOrders = PData;
                    var menudata = context.Menu.Where(x => x.RCode == RCode).ToList();
                    var menuTypes = context.MenuTypes.ToList();
                    PData = PData.Select(x =>
                    {
                        PlaceOrderItem.AddRange(x.PlaceOrderItems.Select(i =>
                        {
                            i.price = i.price - (i.price * Convert.ToInt32(i.Discount) / 100);
                            i.menu = menudata.FirstOrDefault(z => z.MCode == Convert.ToInt32(i.MCode));
                            i.menuType = menuTypes.FirstOrDefault(z => z.MTCode == Convert.ToInt32(i.menu.MTCode)).MTDesc;
                            i.TotalPrice = i.price * i.Quantity;
                            return i;
                        }).ToList());
                        return x;
                    }).ToList();


                    var data = Model.PlaceOrders.Where(a => a.Table == TCode).DistinctBy(x => x.CreatedBy).Select(x => x.CreatedBy).ToList();
                    var DiscountedItemCount = PlaceOrderItem.Where(x => x.Discount != "" && x.Discount != null
                     && x.Discount != "0").ToList().Count();
                    List<UserDetails> users = new List<UserDetails>();

                    foreach (var item in data)
                    {
                        var Newdata = context.User.FirstOrDefault(z => z.MobileNumber == item);
                        UserDetails user = new UserDetails();
                        user.Name = Newdata == null ? item : Newdata.Name != null && Newdata.Name != "" ? Newdata.Name: "";
                        user.Contact = Newdata == null ? item : Newdata.MobileNumber != null && Newdata.MobileNumber != "" ? Newdata.MobileNumber : "";
                        users.Add(user);
                    }
                    Model.Users = users;
                    Model.RCode = RCode.ToString();
                    Model.TCode = TCode;
                    //Bill with all kind of Charges
                    var model = new TotalBillViewModel();
                    model.TotalBill = PlaceOrderItem.Where(x => x.menuType == Constant.Beverage || x.menuType == Constant.NonVeg || x.menuType == Constant.Veg).Select(x => x.TotalPrice).Sum();
                    model.FoodTotalBill = PlaceOrderItem.Where(x => x.menuType == Constant.NonVeg || x.menuType == Constant.Veg).Select(x => x.TotalPrice).Sum();
                    model.BevrageTotalBill = PlaceOrderItem.Where(x => x.menuType == Constant.Beverage).Select(x => x.TotalPrice).Sum();
                    model.BarTotalBill = PlaceOrderItem.Where(x => x.menuType == Constant.Bar).Select(x => x.TotalPrice).Sum();
                    model.restaurant = restaurant;
                    model.TCode = TCode.ToString();
                    model.isDiscountedItem = DiscountedItemCount > 0 ? true : false;
                    model.RCode = RCode.ToString();
                    if (PData.FirstOrDefault(x => x.CouponName != null && x.CouponName != "") != null)
                    {
                        model.CouponCode = PData.FirstOrDefault(x => x.CouponName != null && x.CouponName != "").CouponName;
                    }
                    var BillView = HelperService.Instance.GetBillAmount(model);
                    Model.TotalBill = BillView.SubTotalPrice;
                    Model.DiscountByAdmin = model.Discount;
                    Model.isSingleUser = isSingleuser;
                    //Model.TotalBill = restaurant.TaxApply == true ? Model.PlaceOrders.Select(z => z.TotalPrice).Sum() :
                    //    Model.PlaceOrders.Select(z => z.TotalPrice).Sum() / 100 * tax + Model.PlaceOrders.Select(z => z.TotalPrice).Sum();
                    return PartialView(Model);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        //Orders is not save by Ucode it is saved by TCode, Save order method is also available in CheckoutController.
        public string SaveOrderbyTableNo(string UCode, string RCode, string TCode, int? paymentType, string Discount, string PunchedBy, bool isEbill)
        {
            string result = "false";
            using (var context = new ApplicationDbContext())
            {
                int rCode = Convert.ToInt32(RCode);
                var rest = context.Restaurants.FirstOrDefault(x => x.Id == rCode);
                var PlaceOrder = context.PlaceOrders.Include(x => x.PlaceOrderItems).Where(x => x.RCode == rCode && x.Table == TCode).ToList();
                var UsersMobNum = PlaceOrder.DistinctBy(x => x.CreatedBy).Select(x => x.CreatedBy).ToList();
                var PlaceOrderItem = new List<PlaceOrderItems>();
                var puncheddata = context.CaptainDetails.FirstOrDefault(x => x.UniqueCode == PunchedBy && x.RCode == RCode);
                PunchedBy = puncheddata != null ? puncheddata.Name: "";
                var menudata = context.Menu.Where(x => x.RCode == rCode).ToList();
                var menuTypes = context.MenuTypes.ToList();
                PlaceOrder = PlaceOrder.Select(x =>
                {
                    PlaceOrderItem.AddRange(x.PlaceOrderItems.Select(i =>
                    {
                        i.price = i.price - (i.price * Convert.ToInt32(i.Discount) / 100);
                        i.menu = menudata.FirstOrDefault(z => z.MCode == Convert.ToInt32(i.MCode));
                        i.menuType = menuTypes.FirstOrDefault(z => z.MTCode == Convert.ToInt32(i.menu.MTCode)).MTDesc;
                        i.TotalPrice = i.price * i.Quantity;
                        return i;
                    }).ToList());
                    return x;
                }).ToList();

                var rOrders = context.Orders.Where(x => x.RCode == rCode && x.BillPayed == true).ToList();
                int rLastOrderId = rOrders != null ? rOrders.Count() : 0;
                int currentOrderId = rLastOrderId + 1;
                string CurrentOID = rest.RPrefix + currentOrderId.ToString();
                //Order save gst is not added yet
                Orders orders = new Orders();
                orders.CreatedBy = UCode;
                orders.UserAddress = TCode.ToString() == Constant.HomeDelivery ? PlaceOrder.Where(x => x.Address != string.Empty && x.Address != null).FirstOrDefault()?.Address : "";
                orders.CreatedOn = DateTime.Now;
                orders.BillPayed = true;
                //Elton
                if (TCode == Constant.TakeAway || TCode == Constant.HomeDelivery)
                {
                    orders.isEbill = PlaceOrder.FirstOrDefault().isEbill;
                    isEbill = orders.isEbill;
                }
                else
                {
                    orders.isEbill = isEbill;
                }
                orders.IsActive = true;
                orders.OrderId = CurrentOID;
                orders.Table = TCode.ToString();
                orders.RCode = rCode;
                orders.PunchedBy = PunchedBy;
                orders.PaymentType = paymentType;

                //Bill with all kind of Charges
                var model = new TotalBillViewModel();
                model.TotalBill = PlaceOrderItem.Where(x => x.menuType == Constant.Beverage || x.menuType == Constant.NonVeg || x.menuType == Constant.Veg).Select(x => x.TotalPrice).Sum();
                model.FoodTotalBill = PlaceOrderItem.Where(x => x.menuType == Constant.NonVeg || x.menuType == Constant.Veg).Select(x => x.TotalPrice).Sum();
                model.BevrageTotalBill = PlaceOrderItem.Where(x => x.menuType == Constant.Beverage).Select(x => x.TotalPrice).Sum();
                model.BarTotalBill = PlaceOrderItem.Where(x => x.menuType == Constant.Bar).Select(x => x.TotalPrice).Sum();
                model.restaurant = rest;
                model.Discount = Discount;
                model.TCode = TCode.ToString();
                var BillView = HelperService.Instance.GetBillAmount(model);
                orders.TotalAmount = BillView.SubTotalPrice;
                orders.DiscountAmount = Convert.ToDecimal(BillView.DiscountAmount);
                orders.Discount = BillView.Discount;
                orders.FoodTotalAmount = BillView.FoodTotalBill;
                orders.BevrageTotalAmount = BillView.BevrageTotalBill;
                orders.BarTotalAmount = BillView.BarTotalBill;
                orders.TaxAmount = BillView.TaxAmount;
                orders.VatAmount = BillView.VatAmount;
                orders.ServiceAmount = BillView.ServiceAmount;

                orders.OrderItem = PlaceOrderItem.Select(x => new OrderItem
                {
                    CreatedOn = DateTime.Now,
                    MCode = x.MCode,
                    Price = x.price,
                    IsActive = true,
                    Quantity = x.Quantity,
                    Discount = x.Discount,
                    PunchedBy = x.PunchedBy,
                }).ToList();
                if (orders.OrderItem.Count > 0)
                {
                    context.Orders.Add(orders);
                    context.SaveChanges();
                    PlaceOrderService.DeletePlaceOderByTableNo(rCode, TCode);
                    if (rest.BillPrinter != null && rest.BillPrinter != string.Empty &&
                        orders.Table != Constant.HomeDelivery && orders.Table != Constant.TakeAway)
                    {
                        var saveprintdata = PrintService.Instance.saveprintdata(rCode, CurrentOID, true);
                    }
                }
                //To remove all login user from table 
                foreach (var item in UsersMobNum)
                {
                    TableService.Instance.DeleteTableUSer(item, rCode);
                }
                result = "true";
            }
            return result;
        }
        #endregion
        #region AdminAction
        public ActionResult SaveOrderByAdmin(string UCode,string TCode,string PunchedBy, int paymentType=1)
        {
            bool result = false;
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var rCode = HttpContext.Session["RCode"];
                    if (rCode != null && UCode != string.Empty && UCode != null && TCode != null && TCode != string.Empty)
                    {
                        int tCode = Convert.ToInt32(TCode);
                        string Result = Checkout.SaveOrder(UCode,rCode.ToString(), tCode.ToString(), paymentType,PunchedBy,false);
                        result = true;
                    }
                }
            }
            catch (Exception)
            {
                result = false;
            }
            return Json(result);
        }
        public ActionResult PrintBillByAdmin(string UCode,string TCode,string CurrentOID)
        {
            bool result = false;
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var rCode = HttpContext.Session["RCode"];
                    if (rCode != null && UCode != string.Empty && UCode != null && TCode != null && TCode != string.Empty)
                    {
                        int tCode = Convert.ToInt32(TCode);
                        result = Checkout.PrintByAdmin(UCode,rCode.ToString(), tCode, CurrentOID);
                    }
                }
            }
            catch (Exception)
            {
                result = false;
            }
            return Json(result);
        }
        #endregion
        #region Print
        [HttpPost]
        public bool PrintPlaceOrder()
        {
            bool result = false;
            try
            {
                var rCode = HttpContext.Session["RCode"];
                if (rCode != null)
                {
                    result = PrintService.Instance.GetStatusForPrintPO(rCode.ToString());
                }
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }
        [HttpPost]
        public ActionResult SaveKotForPrint(string PoId)
        {
            bool saveprintdata = false;
            var RCode = HttpContext.Session["RCode"];
            if (RCode != null) {
                int rCode = Convert.ToInt32(RCode.ToString());
                var rest = RestaurantService.Instance.GetRestaurantByID(RCode.ToString());
                if (rest.BillPrinter != null && rest.BillPrinter != string.Empty)
                {
                    saveprintdata = PrintService.Instance.saveprintdata(rCode, PoId, false);
                }
            }
            return Json(saveprintdata);
        }
        public ActionResult SaveBillForPrint(int RCode, string OrderId)
        {
            var saveprintdata = PrintService.Instance.saveprintdata(RCode, OrderId, true);
            return RedirectToAction("Orders", "Admin");
        }
        public ActionResult SaveDemoBillForPrint(int RCode, string PlaceOrderId)
        {
            var saveprintdata = PrintService.Instance.saveprintdata(RCode, PlaceOrderId, true,true);
            return RedirectToAction("CurrentStatus", "Admin");
        }
        #endregion
        #region MenuCategories
        [HttpGet]
        public ActionResult AdminMenuCategories(int id)
        {
            ViewBag.id = id;
            using (var context = new ApplicationDbContext())
            {
                var rest = context.Restaurants.FirstOrDefault(x => x.Id == id);
                ViewBag.RestName = rest.Name;
            }
            var data = SuperAdminService.Instance.getMCatList(id);
            return View(data);
        }
        public ActionResult _AdminMCatAdd(int id)
        {
            var data = new MenuCategory();
            using (var context = new ApplicationDbContext())
            {
                data.RCode = id;
                ViewBag.RestName = context.Restaurants.FirstOrDefault(x => x.Id == id).Name;
                return View(data);
            }
        }
        [HttpPost]
        public ActionResult _AdminMCatAdd(MenuCategory menuCategory)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var catlist = context.MenuCategory.Where(x => x.MCDesc == menuCategory.MCDesc && x.RCode == menuCategory.RCode).ToList();
                    if (catlist.Count > 0)
                    {
                        ViewBag.errmsg = "Category already exists";
                        var list = new List<SelectListItem>();
                        var pack = context.Restaurants.ToList();
                        for (int i = 0; i < pack.Count; i++)
                        {
                            var rdata = new SelectListItem() { Text = pack[i].Name, Value = pack[i].Id.ToString() };
                            list.Add(rdata);
                        }
                        ViewBag.Menus = list;
                        ViewBag.RestName = context.Restaurants.FirstOrDefault(x => x.Id == menuCategory.RCode).Name;
                        return PartialView(menuCategory);
                    }
                    else if (menuCategory.Bar == false && menuCategory.Beverages == false && menuCategory.NonVeg == false && menuCategory.Veg == false)
                    {
                        ViewBag.errmsg = "Choose any one category";
                        var list = new List<SelectListItem>();
                        var pack = context.Restaurants.ToList();
                        for (int i = 0; i < pack.Count; i++)
                        {
                            var rdata = new SelectListItem() { Text = pack[i].Name, Value = pack[i].Id.ToString() };
                            list.Add(rdata);
                        }
                        ViewBag.Menus = list;
                        ViewBag.RestName = context.Restaurants.FirstOrDefault(x => x.Id == menuCategory.RCode).Name;
                        return PartialView(menuCategory);
                    }
                    else
                    {
                        if (!ModelState.IsValid)
                        {
                            return View(menuCategory);
                        }
                        menuCategory.CreatedOn = DateTime.Now;
                        menuCategory.IsActive = true;

                        context.MenuCategory.Add(menuCategory);
                        context.SaveChanges();
                        return RedirectToAction("AdminMenuCategories", new { id = menuCategory.RCode });
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult AdminMcatStatus(int mcId, bool status)
        {
            var result = false;
            using (var context = new ApplicationDbContext())
            {
                var data = context.MenuCategory.FirstOrDefault(x => x.MCCode == mcId);
                data.IsActive = status;
                context.Entry(data).State = EntityState.Modified;
                context.SaveChanges();
                result = true;
            }
            return Json(result);
        }
        public ActionResult _AdminMCatEdit(int id)
        {
            using (var context = new ApplicationDbContext())
            {
                var data = context.MenuCategory.Where(x => x.MCCode == id).FirstOrDefault();
                var list = new List<SelectListItem>();
                var pack = context.Restaurants.ToList();
                for (int i = 0; i < pack.Count; i++)
                {
                    var rdata = new SelectListItem() { Text = pack[i].Name, Value = pack[i].Id.ToString() };
                    list.Add(rdata);
                }
                ViewBag.Menus = list;
                return View(data);
            }
        }

        [HttpPost]
        public ActionResult _AdminMCatEdit(MenuCategory menuCategory)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(menuCategory);

                }
                using (var context = new ApplicationDbContext())
                {
                    context.Entry(menuCategory).State = EntityState.Modified;
                    context.SaveChanges();
                }
                return RedirectToAction("AdminMenuCategories", new { id = menuCategory.RCode });
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion
        #region Menu
        public ActionResult AdminMenu()
        {
            try
            {
                IEnumerable<AdminMenuViewModel> data = null;
                var rCode = HttpContext.Session["RCode"];
                if (rCode != null)
                {
                    int RCode = Convert.ToInt32(rCode);
                    data = MenuDetailsService.Instance.GetRestMenusByRCode(RCode);
                    ViewBag.Rcode = RCode;
                }
                return View(data);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost]
        public ActionResult AdminMenuStatus(int menuId, bool status)
        {
            var result = false;
            using (var context = new ApplicationDbContext())
            {
                var data = context.Menu.FirstOrDefault(x => x.MCode == menuId);
                data.IsActive = status;
                context.Entry(data).State = EntityState.Modified;
                context.SaveChanges();
                result = true;
            }
            return Json(result);
        }
        public ActionResult _AdminMenuAdd(int rid)
        {
            using (var context = new ApplicationDbContext())
            {
                var tlist = new List<SelectListItem>();
                var type = context.MenuTypes.Where(x => x.IsActive == true).OrderByDescending(x => x.MTCode).ToList();
                for (int i = 0; i < type.Count; i++)
                {
                    var tdata = new SelectListItem() { Text = type[i].MTDesc, Value = type[i].MTCode.ToString() };
                    tlist.Add(tdata);
                }
                var catList = new List<SelectListItem>();
                var category = context.MenuCategory.Where(x => x.RCode == rid && x.IsActive == true).OrderByDescending(x => x.MCCode).ToList();
                for (int i = 0; i < category.Count; i++)
                {
                    var catData = new SelectListItem() { Text = category[i].MCDesc, Value = category[i].MCCode.ToString() };
                    catList.Add(catData);
                }
                ViewBag.menuCatItem = catList;
                ViewBag.tp = tlist;
                ViewBag.rid = rid;
                ViewBag.RestName = context.Restaurants.FirstOrDefault(x => x.Id == rid).Name;
                return View();
            }
        }
        [HttpPost]
        public ActionResult _AdminMenuAdd(Menu menu)
        {
            using (var context = new ApplicationDbContext())
            {
                var repmenu = context.Menu.Where(x => x.MDesc == menu.MDesc && x.RCode == menu.RCode);
                var tlist = new List<SelectListItem>();
                var type = context.MenuTypes.Where(x => x.IsActive == true).OrderByDescending(x => x.MTCode).ToList();
                for (int i = 0; i < type.Count; i++)
                {
                    var tdata = new SelectListItem() { Text = type[i].MTDesc, Value = type[i].MTCode.ToString() };
                    tlist.Add(tdata);
                }
                var catList = new List<SelectListItem>();
                var category = context.MenuCategory.Where(x => x.RCode == menu.RCode && x.IsActive == true).OrderByDescending(x => x.MCCode).ToList();
                for (int i = 0; i < category.Count; i++)
                {
                    var catData = new SelectListItem() { Text = category[i].MCDesc, Value = category[i].MCCode.ToString() };
                    catList.Add(catData);
                }
                ViewBag.menuCatItem = catList;
                ViewBag.tp = tlist;
                ViewBag.rid = menu.RCode;
                ViewBag.RestName = context.Restaurants.FirstOrDefault(x => x.Id == menu.RCode).Name;
                if (repmenu.Count() > 0)
                {
                    ViewBag.ErrMsg = "Menu Item already exists";
                    return View(menu);
                }
                else
                {
                    if (!ModelState.IsValid)
                    {
                        return View(menu);
                    }
                    menu.CreatedOn = DateTime.Now;
                    menu.IsActive = true;
                    menu.Rating = menu.Rating != null && menu.Rating != string.Empty ? menu.Rating : "3";
                    context.Menu.Add(menu);
                    context.SaveChanges();
                    return RedirectToAction("AdminMenu");

                }
            }
        }
        public ActionResult _AdminMenuEdit(int id)
        {
            using (var context = new ApplicationDbContext())
            {
                var data = context.Menu.FirstOrDefault(x => x.MCode == id);
                var list = new List<SelectListItem>();
                var menu = context.MenuCategory.Where(x => x.RCode == data.RCode && x.IsActive == true).ToList();
                for (int i = 0; i < menu.Count; i++)
                {
                    var rdata = new SelectListItem() { Text = menu[i].MCDesc, Value = menu[i].MCCode.ToString() };
                    list.Add(rdata);
                }
                var tlist = new List<SelectListItem>();
                var type = context.MenuTypes.OrderByDescending(x => x.MTCode).ToList();
                for (int i = 0; i < type.Count; i++)
                {
                    var tdata = new SelectListItem() { Text = type[i].MTDesc, Value = type[i].MTCode.ToString() };
                    tlist.Add(tdata);
                }
                if (data.img != null && data.img != string.Empty) { TempData["OldImage"] = data.img; }
                ViewBag.tp = tlist;
                ViewBag.cat = list;
                return View(data);
            }
        }
        [HttpPost]
        public ActionResult _AdminMenuEdit(Menu menu)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(menu);
                }
                using (var context = new ApplicationDbContext())
                {
                    context.Entry(menu).State = EntityState.Modified;
                    context.SaveChanges();
                    if (TempData["OldImage"] != null)
                    {
                        if (TempData["OldImage"].ToString() != menu.img)
                        {
                            string removeimagepath = Request.MapPath(TempData["OldImage"].ToString());
                            if (System.IO.File.Exists(removeimagepath))
                            {
                                System.IO.File.Delete(removeimagepath);
                            }
                        }
                    }
                }
                return RedirectToAction("AdminMenu");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public string MenuDiscount(int Id,string Discount, string TADiscount , string HDDiscount)
        {
            var result = "false";
            try
            {
                var rCode = HttpContext.Session["RCode"];
                int RCode = Convert.ToInt32(rCode);
                using(var context =new ApplicationDbContext())
                {
                    var data = context.Menu.FirstOrDefault(x => x.MCode == Id && x.RCode == RCode);
                    if (data != null && Discount != null)
                    {
                        data.Discount = Discount;
                        data.TADiscount = TADiscount;
                        data.HDDiscount = HDDiscount;
                        context.Entry(data).State = EntityState.Modified;
                        context.SaveChanges();
                        result = "true";
                    }
                }
            }
            catch (Exception ex)
            {
                result = "false";
            }
            return result;
        }
        #endregion
        #region Inventory
        public ActionResult InventoryTable()
        {
            var data = InventoryService.Instance.GetInventoryData();
            return View(data);
        }
        public ActionResult InventoryAdd()
        {
            return View();
        }
        [HttpPost]
        public ActionResult InventoryAdd(Inventory inventory)
        {
            inventory.DateTime = DateTime.Now.ToString();
            if (!ModelState.IsValid)
            {
                return View(inventory);
            }
            InventoryService.Instance.InventoryAdd(inventory);
            return RedirectToAction("InventoryTable");
        }
        public ActionResult InventoryEdit(int Id)
        {
            var data = InventoryService.Instance.GetInventoryDataById(Id);
            return View(data);
        }
        [HttpPost]
        public ActionResult InventoryEdit(Inventory inventory)
        {
            inventory.DateTime = DateTime.Now.ToString();
            if (!ModelState.IsValid)
            {
                return View(inventory);
            }
            InventoryService.Instance.InventoryEdit(inventory);
            return RedirectToAction("InventoryTable");
        }
        [HttpPost]
        public ActionResult InventoryItemStatus(int Id, bool status)
        {
            var result = false;
            using (var context = new ApplicationDbContext())
            {
                var data = context.Inventories.FirstOrDefault(x => x.Id == Id);
                data.Status = status;
                context.Entry(data).State = EntityState.Modified;
                context.SaveChanges();
                result = true;
            }
            return Json(result);
        }
        public ActionResult ListInventory()
        {
            return View();
        }
        
        public ActionResult AddItemInventory()
        {
            try
            {
                //empty 
                return View();
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion
        #region CaptainService_&_Details
        public ActionResult addItemForUser(string Rcode, string TCode, string UCode,bool isPlacedOrder = false)
        {
            try
            {
                var Model = new AddItemByAdmin();
                Model.Menus = MenuDetailsService.Instance.GetRestMenus(Convert.ToInt32(Rcode));
                Model.MenuCategories = MenuDetailsService.Instance.GetRestMenuCategory(Convert.ToInt32(Rcode));
                Model.TCode = TCode;
                Model.RCode = Rcode;
                Model.UCode = UCode;
                Model.isNew = true;
                Model.isPlacedOrder = isPlacedOrder;
                if (UCode != string.Empty && UCode != null)
                {
                    Model.isNew = false;
                }
                return View(Model);
            }
            catch (Exception)
            {
                throw;
            }
        }
        //save order by captain or admin
        [HttpPost]
        public ActionResult savePlaceOrderData(AdminPlaceOrderModel model)
        {
            bool result = false;
            if (model.MCode != null && model.MCode != "")
            {
                var placeOrderItemsList = model.MCode.Split(',');
                var discItemsList = model.MCode.Split(',');
                int RCode = Convert.ToInt32(model.RestCode);
                var mCode = MenuDetailsService.Instance.GetRestMenus(Convert.ToInt32(model.RestCode));
                using (var context = new ApplicationDbContext())
                {
                    var puncheddata = context.CaptainDetails.FirstOrDefault(x => x.UniqueCode == model.PunchedBy && x.RCode == model.RestCode);
                    model.PunchedBy = puncheddata != null ? puncheddata.Name : "";
                    var rest = context.Restaurants.FirstOrDefault(x => x.Id == RCode);
                    PlaceOrder placeOrder = new PlaceOrder()
                    {
                        Table = model.TableCode,
                        Comment = model.Comments,
                        CreatedBy = model.CreatedBy,
                        CreatedOn = DateTime.Now,
                        IsActive = true,
                        isOld = true,
                        RCode = RCode,
                        PlaceOrderItems = placeOrderItemsList.Select(x => new PlaceOrderItems()
                        {
                            MCode = Convert.ToInt32(x.Substring(0, x.IndexOf('/'))),
                            Quantity = Convert.ToInt32(x.Substring(0, x.IndexOf('-')).Remove(0, Convert.ToString(x.Substring(0, x.IndexOf('/'))).Length + 1)),
                            price = mCode.FirstOrDefault(y => y.MCode == Convert.ToInt32(x.Substring(0, x.IndexOf('/')))).Price,
                            Discount = MenuDetailsService.Instance.GetDiscountPunchedBy(RCode, Convert.ToInt32(x.Substring(0, x.IndexOf('/'))), x.Substring(x.IndexOf('-')).Remove(0, 1), model.PunchedBy, model.TableCode),
                            PunchedBy = MenuDetailsService.Instance.CheckPunchedBy(RCode, Convert.ToInt32(x.Substring(0, x.IndexOf('/'))), x.Substring(x.IndexOf('-')).Remove(0, 1), model.PunchedBy, model.TableCode)
                        }).ToList(),
                    };
                    context.PlaceOrders.Add(placeOrder);
                    context.SaveChanges();
                    if (rest.BillPrinter != null && rest.BillPrinter != string.Empty)
                    {
                        var saveprintdata = PrintService.Instance.saveprintdata(RCode, Convert.ToString(placeOrder.Id), false);
                    }
                    if (model.isNew == true)
                    {
                        TableService.Instance.BookTableByQrCode(model.CreatedBy, model.TableCode, model.RestCode);
                    }
                    result = true;
                    MyHub objNotifHub = new MyHub();
                    objNotifHub.PlaceOrderData(RCode.ToString(), placeOrder.Id.ToString());
                }
            }
            return Json(result);
        }
        public JsonResult getMenuData(int MCCode,string TCode)
        {
            try
            {
                IEnumerable<DropdownMenuViewModel> data = null;
                var rCode = HttpContext.Session["RCode"];
                if (rCode != null)
                {
                    int RCode = Convert.ToInt32(rCode);
                    data = MenuDetailsService.Instance.DropdownMenu(RCode, MCCode, TCode);
                }
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }
        //captain details save 
        public ActionResult CaptainTable()
        {
            try
            {
                var rCode = HttpContext.Session["RCode"];
                var data = CaptainService.Instance.RestCaptainsList(rCode.ToString());
                return View(data);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public ActionResult AddCaptain()
        {
            var rCode = HttpContext.Session["RCode"];
            ViewBag.RCode = rCode.ToString();
            return View();
        }
        [HttpPost]
        public ActionResult AddCaptain(CaptainDetails Model)
        {
            
            Model.CreatedBy = "Admin";
            Model.Address = Model.Address != null ? Model.Address : "-";
            Model.Email = Model.Email != null ? Model.Email : "-";
            Model.CreatedOn = DateTime.Now;
            Model.IsActive = true;
            if (!ModelState.IsValid)
            {
                return View(Model);
            }
            var CId = CaptainService.Instance.CheckCId(Model.UniqueCode,Model.RCode);
            if (CId == true)
            {
                TempData["Message"] = "* Unique Code for captain is already used";
                return View(Model);
            }
            var data = CaptainService.Instance.GetAddCaptain(Model);
            TempData["Message"] = data;
            return RedirectToAction("CaptainTable");
        }
        public ActionResult EditCaptain(int Id)
        {
            try
            {
                var data = CaptainService.Instance.GetCaptain(Id);
                return View(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public ActionResult EditCaptain(CaptainDetails details)
        {
            try
            {
                details.CreatedBy = "Admin";
                details.CreatedOn = DateTime.Now;
                details.IsActive = true;
                if (!ModelState.IsValid)
                {
                    return View(details);
                }
                var data = CaptainService.Instance.GetEditCaptain(details);
                TempData["Message"] = data;
                return RedirectToAction("CaptainTable");
            }
            catch (Exception)
            {
                throw;
            }
        }
        public ActionResult DeleteCaptain(int Id)
        {
            var data = CaptainService.Instance.GetDeleteCaptain(Id);
            TempData["Message"] = data;
            return RedirectToAction("CaptainTable");
        }
        public string CaptainCodeCheck(string Code)
        {
            using(var context = new ApplicationDbContext())
            {
                var rCode = HttpContext.Session["RCode"];
                var rcode = rCode.ToString();
                var result = "false";
                var data = context.CaptainDetails.FirstOrDefault(x=>x.RCode == rcode && x.UniqueCode == Code);
                if (data != null)
                {
                    result = "true";
                }
                return result;
            }
        }
        #endregion
        #region BillDiscount & Promotional
        public ActionResult ListBillDiscount()
        {
            try
            {
                var rCode = HttpContext.Session["RCode"];
                if (rCode != null)
                {
                    var data = BillDiscountService.Instance.GetListBillDiscount(rCode.ToString());
                    return View(data);
                }
                else
                {
                    return RedirectToAction("SuperAdminLogin", "AdminAccount");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public ActionResult AddBillDiscount()
        {
            var rCode = HttpContext.Session["RCode"].ToString();
            var data =new BillDiscount();
            data.isDiscount = true;
            data.RCode = rCode.ToString();
            data.CreatedOn = DateTime.Now;
            var menuType = MenuDetailsService.Instance.GetMenuType();
            var CompleteMenu = MenuDetailsService.Instance.GetRestMenus(Convert.ToInt32(data.RCode));
            bool Bar = CompleteMenu.FirstOrDefault(x => x.MTCode == menuType.FirstOrDefault(y => y.MTDesc == Constant.Bar).MTCode) != null ? true : false;
            bool Veg = CompleteMenu.FirstOrDefault(x => x.MTCode == menuType.FirstOrDefault(y => y.MTDesc == Constant.Veg).MTCode) != null ? true : false;
            bool NonVeg = CompleteMenu.FirstOrDefault(x => x.MTCode == menuType.FirstOrDefault(y => y.MTDesc == Constant.NonVeg).MTCode) != null ? true : false;
            bool Beverages = CompleteMenu.FirstOrDefault(x => x.MTCode == menuType.FirstOrDefault(y => y.MTDesc == Constant.Beverage).MTCode) != null ? true : false;
            data.isFoodAvail = false;
            data.isBarAvail = false;
            if (Veg == true || NonVeg == true || Beverages == true)
            {
                data.isFoodAvail = true;
            }
            if (Bar == true)
            {
                data.isBarAvail = true;
            }
            return View(data);
        }
        [HttpPost]
        public ActionResult AddBillDiscount(BillDiscount model)
        {
            bool result = false;
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                if (model.Table == false && model.TakeAway == false && model.HomeDelivery == false)
                {
                    TempData["Message"] = "* Please select any Discout On.";
                    return View(model);
                }
                if (model.DiscountAmount == "0" && model.DiscountPercentage == "0")
                {
                    TempData["Message"] = "* Please add any one from Discount Amount or Discount Percentage.";
                    return View(model);
                }
                if (model.isDiscount == false && model.CouponName == "" && model.CouponName == null)
                {
                    TempData["Message"] = "* Please add coupon name.";
                    return View(model);
                }
                if (model.isDiscount == false && model.CouponName != null)
                {
                    var CouponData = BillDiscountService.Instance.GetCouponList(model.RCode);
                    if (CouponData.FirstOrDefault(x=>x.CouponName == model.CouponName) != null)
                    {
                        TempData["Message"] = "* Coupon is already added.";
                        return View(model);
                    }
                }
                result = BillDiscountService.Instance.checkandSaveBillDis(model);
            }
            catch (Exception)
            {
                result = false;
            }
            TempData["Message"] = result;
            return RedirectToAction("ListBillDiscount");
        }
        public ActionResult RemoveBillDiscount(int Id)
        {
            var rCode = HttpContext.Session["RCode"];
            if (rCode != null && Id > 0)
            {
                var result = BillDiscountService.Instance.RemoveBillDiscount(rCode.ToString(), Id);
            }
            else
            {
                return RedirectToAction("SuperAdminLogin", "AdminAccount");
            }
            return RedirectToAction("ListBillDiscount");
        }
        public ActionResult Promotional(string Sub, string Head, string Body, string Image)
        {
            string result = "false";
            try
            {
                var RId = Convert.ToString(HttpContext.Session["RCode"]);
                int RCode = Convert.ToInt32(RId);
                var restaurant = RestaurantService.Instance.GetRestaurantByID(RId);
                string dName = HttpContext.Request.Url.GetLeftPart(UriPartial.Authority);
                restaurant.Img = dName + restaurant.Img;
                var alluser = UserService.Instance.GetUsers();
                var restuser = new List<UserCount>();
                using (var context = new ApplicationDbContext())
                {
                    var RestOrders = context.Orders.Include(x => x.OrderItem).Where(x => x.RCode == RCode).ToList();
                    restuser = RestOrders.Select(x => new UserCount()
                    {
                        User = alluser.FirstOrDefault(b => b.MobileNumber == x.CreatedBy || b.Email == x.CreatedBy) != null && alluser.FirstOrDefault(b => b.MobileNumber == x.CreatedBy || b.Email == x.CreatedBy).Email != null ? alluser.FirstOrDefault(b => b.MobileNumber == x.CreatedBy || b.Email == x.CreatedBy).Email : "",
                        Count = RestOrders.Where(z => z.CreatedBy == x.CreatedBy).Count()
                    }).DistinctBy(a => a.User).ToList();

                }

                foreach (var email in restuser)
                {
                    NewsletterViewModel nm = new NewsletterViewModel();
                    nm.subject = Sub;
                    nm.head = Head;
                    nm.body = Body;
                    nm.img = Image;
                    nm.email = email.User;
                    nm.Restaurant = restaurant;
                    //promomail(nm);
                }

                result = "true";

            }
            catch (Exception)
            {
                result = "false";
                throw;
            }
            return Json(new { result = result });
        }
        #endregion
        #region SignalR
        [HttpPost]
        public JsonResult GetCompleteOrders()
        {
            var rCode = HttpContext.Session["RCode"];
            int RCode = Convert.ToInt32(rCode);
            DateTime Today = DateTime.Now;
            int count = 0;
            var listOrder = new List<Orders>();
            //var notificationRegisterTime = Session["LastTimeNotified"] != null ? Convert.ToDateTime(Session["LastTimeNotified"]) : DateTime.Now;
            using (var context = new ApplicationDbContext())
            {
                var query = (from t in context.Orders where t.RCode == RCode && t.CreatedOn.Year == Today.Year && t.CreatedOn.Month == Today.Month && t.CreatedOn.Day == Today.Day select t).ToList();
                count = query.Count;
            }
            //UPDATE SESSION FOR GETTING NEWLY ADDED INFORMATION ONLY
            //  Session["LastTimeNotified"] = DateTime.Now;
            return new JsonResult { Data = count, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        [HttpPost]
        public JsonResult GetPlaceOrdersData()
        {
            var rCode = HttpContext.Session["RCode"];
            int RCode = Convert.ToInt32(rCode);
            DateTime Today = DateTime.Now;
            int count = 0;
            var listOrder = new List<PlaceOrder>();
            //var notificationRegisterTime = Session["LastTimeNotified"] != null ? Convert.ToDateTime(Session["LastTimeNotified"]) : DateTime.Now;
            using (var context = new ApplicationDbContext())
            {
                var query = (from t in context.PlaceOrders where t.RCode == RCode && t.CreatedOn.Year == Today.Year && t.CreatedOn.Month == Today.Month && t.CreatedOn.Day == Today.Day select t).ToList();
                count = query.Count;
            }

            //UPDATE SESSION FOR GETTING NEWLY ADDED INFORMATION ONLY
            //  Session["LastTimeNotified"] = DateTime.Now;
            return new JsonResult { Data = count, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        #endregion
    }
}
