using DataBase;
using Models;
using Newtonsoft.Json;
using Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using ViewModels;

namespace HotelAutomation.Web.Controllers
{
    public class CheckoutController : Controller
    {
        CartController cartController = new CartController();
        APIController API = new APIController();
        [Route("UserDetails")]
        [HttpGet]
        public ActionResult UserDetails(string Coupon)
        {
            try
            {
                var model = new CheckOutViewModel();
                var loginUser = UserService.Instance.GetCurrentUserLogin();
                if (loginUser.isLogin == false) { return RedirectToAction("Login", "Account"); }
                model.TableNo = loginUser.TCode;
                model.UserDetail = UserService.Instance.GetUserInfo(loginUser.UCode);
                model.RestaurantDetail = RestaurantService.Instance.GetRestaurantByID(loginUser.RCode);
                if (Coupon != null && Coupon != "")
                {
                    Coupon = HelperService.Instance.Decrypt(Coupon);
                    PlaceOrderService.Instance.AddCouponName(Coupon, loginUser.UCode, loginUser.RCode, loginUser.TCode);
                }
                return View(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Error", new { CloseWeb = false, maintainance = false, NotFound = true });
            }
        }
        [HttpPost]
        public ActionResult UserDetails(User user)
        {
            bool Result = false;
            string OrderId = "";
            try
            {
                var loginUser = UserService.Instance.GetCurrentUserLogin();
                if (loginUser.isLogin == false) { return RedirectToAction("Login", "Account"); }
                var rCode = Convert.ToInt32(loginUser.RCode);
                if (loginUser.TCode == Constant.HomeDelivery || loginUser.TCode == Constant.TakeAway)
                {
                    using(var context = new ApplicationDbContext())
                    {
                        PlaceOrder placeOrder = context.PlaceOrders.FirstOrDefault(x => x.Table == loginUser.TCode && x.RCode == rCode && x.CreatedBy == loginUser.UCode);
                        if (loginUser.TCode == Constant.HomeDelivery)
                        {
                            placeOrder.Address = user.HouseNo + " " + user.LocalArea + " " + user.LandMark + " " + user.Street + " " + user.Pincode;
                        }
                        placeOrder.isEbill = user.isEbill;
                        context.Entry(placeOrder).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();
                    }
                }
                Result = true;
                if (loginUser.TCode != Constant.HomeDelivery && loginUser.TCode != Constant.TakeAway)
                {
                    // SaveOrder(uCode, rCode.ToString(), tCode.ToString(), user.PaymentType,"0","");
                    OrderId = SaveOrder(loginUser.UCode, rCode.ToString(), loginUser.TCode.ToString(), user.PaymentType, "0", user.isEbill, "");
                    if (OrderId != "" && OrderId != null)
                    {
                        OrderId = HelperService.Instance.ConvertStringToHex(OrderId, System.Text.Encoding.Unicode);
                    }
                }
                var userData = UserService.Instance.GetUserInfo(loginUser.UCode);
                if (user.isChange == true || userData.Email != user.Email)
                {
                     UserService.Instance.SaveUserData(user).ToString();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(new {result =  Result, orderId = OrderId }, JsonRequestBehavior.AllowGet);
        }
        //Orders is not save by TCode it is saved by UCode, Save order method is also available in AdminController by name SaveOrderbyTableNo.
        public string SaveOrder(string UCode, string RCode, string TCode, int? paymentType, string PunchedBy, bool isEbill, string Discount = null)
        {
            var result = false;
            string OrderId = "";
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    int rCode = Convert.ToInt32(RCode);
                    var rest = context.Restaurants.FirstOrDefault(x => x.Id == rCode);
                    var PlaceOrder = context.PlaceOrders.Include(x => x.PlaceOrderItems).Where(x => x.RCode == rCode && x.CreatedBy == UCode && x.Table == TCode).ToList();
                    var PlaceOrderItem = new List<PlaceOrderItems>();
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
                    var DiscountedItemCount = PlaceOrderItem.Where(x => x.Discount != "" && x.Discount != null
                    && x.Discount != "0").ToList().Count();
                    var puncheddata = context.CaptainDetails.FirstOrDefault(x => x.UniqueCode == PunchedBy && x.RCode == RCode);
                    PunchedBy = puncheddata != null ? puncheddata.Name : "";
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
                    orders.IsActive = true;
                    orders.OrderId = CurrentOID;
                    orders.Table = TCode.ToString();
                    orders.RCode = rCode;
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
                    model.RCode = RCode;
                    model.isDiscountedItem = DiscountedItemCount > 0 ? true : false;
                    if (PlaceOrder.FirstOrDefault(x=>x.CouponName != null || x.CouponName != "") != null)
                    {
                        model.CouponCode = PlaceOrder.FirstOrDefault().CouponName;
                    }
                    var BillView = HelperService.Instance.GetBillAmount(model);
                    orders.TotalAmount = BillView.SubTotalPrice;
                    orders.DiscountAmount = Convert.ToDecimal(BillView.DiscountAmount);
                    if (BillView.CouponCode != null)
                    {
                        orders.CouponName = BillView.CouponCode;
                        orders.CouponDiscount = BillView.CouponDiscount;
                        orders.CouponAmount = Convert.ToDecimal(BillView.CouponDiscountAmount);
                        var CouponDiscount = BillDiscountService.Instance.GetCoupon(model.RCode, model.CouponCode);
                        if (CouponDiscount != null && CouponDiscount.isSingleUse == true)
                        {
                            HelperService.Instance.AddUserInSingleCoupon(model.CouponCode, model.RCode, UCode);
                        }
                    }
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
                        PunchedBy = x.PunchedBy,
                        Discount = x.Discount
                    }).ToList();
                    if (orders.OrderItem.Count > 0)
                    {
                        context.Orders.Add(orders);
                        OrderId = orders.OrderId;
                        context.SaveChanges();
                        PlaceOrderService.DeletePlaceOder(UCode, rCode, TCode);
                        if (rest.BillPrinter != null && rest.BillPrinter != string.Empty &&
                            orders.Table != Constant.HomeDelivery && orders.Table != Constant.TakeAway)
                        {
                            var saveprintdata = PrintService.Instance.saveprintdata(rCode, CurrentOID, true);
                        }
                        if (isEbill == true)
                        {
                            ebillMail(orders.RCode.ToString(), orders.CreatedBy, CurrentOID);
                        }
                    }
                    TableService.Instance.DeleteTableUSer(UCode, rCode);
                    result = true;
                    if (result == true)
                    {
                        MyHub objNotifHub = new MyHub();
                        objNotifHub.TotalOrderData(RCode, OrderId);
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
            }
            return OrderId;

        }
        //public ActionResult Bill(string UCode, string RCode, string TCode)
        //{
        //    try
        //    {
        //        var billviewdata = new List<RootObject>();
        //        var billdata = new BillPrintOrder();
        //        var WebCookie = Request.Cookies[Constant.WebCookie];
        //        if (WebCookie == null) { return RedirectToAction("Login", "Account"); }
        //        var userDetails = HelperService.Instance.GetDetailByCookie(WebCookie.Value);
        //        billdata.RCode = Convert.ToInt32(userDetails.RCode);
        //        billdata.isBill = true;
        //        using (var context = new ApplicationDbContext())
        //        {
        //            var order = context.Orders.OrderByDescending(x => x.CreatedOn).FirstOrDefault(x => x.RCode == billdata.RCode && x.Table == userDetails.TCode
        //              && x.CreatedBy == userDetails.UCode);
        //            billdata.OrderId = order.OrderId;
        //            var bill = PrintService.Instance.GetPrintData(billdata);
        //            var data = JsonConvert.SerializeObject(bill);
        //            billviewdata = JsonConvert.DeserializeObject<List<RootObject>>(data);
        //            int rCode = Convert.ToInt32(RCode);
        //            var rest = context.Restaurants.FirstOrDefault(x => x.Id == rCode);
        //        }
        //        return PartialView(billviewdata);
        //    }//
        //    catch (Exception ex)
        //    {
        //        return PartialView(ex);
        //    }
        //}
        [Route("ThankYouPage")]
        public ActionResult ThankYouPage(string OrderId)
        {
            try
            {
                var loginUser = UserService.Instance.GetCurrentUserLogin();
                if (loginUser.isLogin == false) { return RedirectToAction("Login", "Account"); }
                int TCode = Convert.ToInt32(loginUser.TCode);
                TempData["Table"] = TCode;
                ViewBag.ReviewUrl = RestaurantService.Instance.GetRestaurantByID(loginUser.RCode) != null ? RestaurantService.Instance.GetRestaurantByID(loginUser.RCode).ReviewUrl:"";
                var UserDetails = UserService.Instance.GetUserInfo(loginUser.UCode);
                ViewBag.UserName = UserDetails != null && UserDetails.Name != null && UserDetails.Name != "" ? UserDetails.Name:"";
                if (TCode.ToString() != Constant.HomeDelivery && TCode.ToString() != Constant.TakeAway && Request.Cookies[Constant.WebCookie] != null)
                {
                    HttpCookie oldcookie = new HttpCookie(Constant.WebCookie);
                    oldcookie.Expires = DateTime.Now.AddDays(-1);
                    Response.Cookies.Add(oldcookie);
                    TableService.Instance.DeleteTableUSer(loginUser.UCode, Convert.ToInt32(loginUser.RCode));
                }
                TempData["OrderId"] = OrderId;
                return View(UserDetails);
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "Error", new { CloseWeb = true, maintainance = false, NotFound = false });
            }
        }
        public bool PrintByAdmin(string UCode, string RCode, int TCode,string CurrentOID)
        {
            bool result = false;
            using (var context = new ApplicationDbContext())
            {
                int rCode = Convert.ToInt32(RCode);
                var rest = context.Restaurants.FirstOrDefault(x => x.Id == rCode);
                //if (rest.BillPrinter != null && rest.BillPrinter != "")
                //{
                //    PrintService.Instance.PrinterBtn(null, rCode, UCode, CurrentOID);
                //}
                if (rest.BillPrinter != null && rest.BillPrinter != string.Empty)
                {
                    var saveprintdata = PrintService.Instance.saveprintdata(rCode, CurrentOID, true);
                }
                result = true;
            }
            return result;
        }
        //Elton
        public ActionResult EBill(string encbid)
        {
            try
            {
                var BId = HelperService.Instance.ConvertHexToString(encbid, System.Text.Encoding.Unicode);
                var EBillData = new EBillModel();
                using (var context = new ApplicationDbContext())
                {
                    var order = context.Orders.Include(x => x.OrderItem).FirstOrDefault(x => x.OrderId == BId);
                    int rid = order.RCode;
                    var Restaurant = context.Restaurants.FirstOrDefault(x => x.Id == rid);
                    EBillData.Orders = order;
                    List<OrderItem> items = new List<OrderItem>();
                    items = order.OrderItem;
                    var menudata = context.Menu.Where(x => x.RCode == rid).ToList();
                    var menuTypes = context.MenuTypes.ToList();

                    order.OrderItem = items.Select(x=> new OrderItem(){ 
                       CreatedOn=x.CreatedOn,
                       Quantity = x.Quantity,
                       CreatedBy = x.CreatedBy,
                       Discount = x.Discount,
                       MCode = x.MCode,
                       Orders = x.Orders,
                       Id = x.Id,
                       OrderId = x.OrderId,
                       menu = menudata.FirstOrDefault(z => z.MCode == Convert.ToInt32(x.MCode)),
                       menuType = menuTypes.FirstOrDefault(z => z.MTCode == Convert.ToInt32(menudata.FirstOrDefault(a => a.MCode == Convert.ToInt32(x.MCode)).MTCode)).MTDesc,
                       Price = x.Price,
                       TotalPrice = x.Price * x.Quantity,
                    }).ToList();

                    EBillData.Restaurant = Restaurant;
                    EBillData.Total = order.OrderItem.Select(x => x.Price * x.Quantity).Sum().ToString();
                    //if (Restaurant.TaxApply == true)
                    //{
                    //    var discount = order.Discount != "0" ? (order.TotalAmount * Convert.ToInt32(order.Discount) / 100).ToString() : "0";
                    //    EBillData.DiscountAmt = discount;
                    //    var total = order.TotalAmount + Convert.ToDecimal(discount);
                    //    EBillData.Total = total.ToString();
                    //}
                    //else
                    //{
                    //    var discount = order.Discount != "0" ? (order.TotalAmount * Convert.ToInt32(order.Discount) / 100).ToString() : "0";
                    //    EBillData.DiscountAmt = discount;
                    //    var total = order.TotalAmount + Convert.ToDecimal(discount);
                    //    EBillData.Total = total.ToString();
                    //}
                }
                return View(EBillData);
            }
            catch (Exception ex)
            {

                return RedirectToAction("ErrorPage", "Error");
            }
        }
        public void ebillMail(string RId, string ucode, string BId)
        {
            HelperController controller = new HelperController();
            try
            {
                var restaurant = RestaurantService.Instance.GetRestaurantByID(RId);
                var encbid = HelperService.Instance.ConvertStringToHex(BId, System.Text.Encoding.Unicode);
                string dName = HttpContext.Request.Url.GetLeftPart(UriPartial.Authority) + "/Checkout/EBill/?encbid="+encbid;
                using (var context = new ApplicationDbContext())
                {
                    var user = context.User.FirstOrDefault(x => x.MobileNumber == ucode || x.Email == ucode);
                    if (user != null && user.Email != null && user.Email != string.Empty)
                    {
                        var order = context.Orders.Include(x => x.OrderItem).FirstOrDefault(x => x.OrderId == BId);
                        string subject = restaurant.Name + ": Ebill for " + order.CreatedOn.ToString("dd/MM/yyyy");
                        string Head = "Thank You for choosing an E-Bill";
                        string body = "<h2>By choosing an E-Bill today helped us save a tree</h2>" +
                            "<p>You can click on the below link to view your E-Bill" +
                            "</br><a href=\"" + dName + "\">Click here for the bill</a></p>";
                        controller.templateEmail(user.Email, subject, Head, body, restaurant.Mobile, restaurant.Address);
                    }

                }
            }
            catch (Exception ex)
            {
               throw ex;
            }
        }
    }
}