using DataBase;
using Models;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ViewModels;
using System.Data.Entity;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using System.Threading.Tasks;

namespace HotelAutomation.Web.Controllers
{
    public class PlaceOrderController : Controller
    {
        // GET: PlaceOrder
        [Route("PlaceOrder")]
        public ActionResult PlaceOrder()
        {
            try
            {
                var loginUser = UserService.Instance.GetCurrentUserLogin();
                if (loginUser.isLogin == false)
                {
                    return RedirectToAction("ErrorPage", "Error", new { CloseWeb = true, maintainance = false, NotFound = false });
                }
                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Error", new { CloseWeb = false, maintainance = false, NotFound = true });
            }
        }
        [HttpPost]
        public bool PlaceOrderAction(string Comment)
        {
            bool result = false;
            try
            {
                var loginUser = UserService.Instance.GetCurrentUserLogin();
                if (loginUser.isLogin == false){ return result; }
                var Result = PlaceOrderService.Instance.PlaceOrderAction(Comment, Convert.ToInt32(loginUser.RCode), loginUser.UCode, Convert.ToInt32(loginUser.TCode));
                if (Result != "" && Result != null)
                {
                    result = true;
                    MyHub objNotifHub = new MyHub();
                    objNotifHub.PlaceOrderData(loginUser.RCode, Result);
                }
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }

        [HttpPost]
        public JsonResult getPlaceOrderData(string CouponName,bool removeCoupon)
        {
            var DataModel = new PlaceOrderViewModel();
            try
            {
                var loginUser = UserService.Instance.GetCurrentUserLogin();
                if (loginUser.isLogin == false)
                {
                    return Json(DataModel, JsonRequestBehavior.AllowGet);
                }
                string TCode = loginUser.TCode;
                int rCode = Convert.ToInt32(loginUser.RCode);
                var UserAdmin = TableService.Instance.IsUserAdmin(loginUser.UCode, loginUser.RCode, loginUser.TCode);
                var PlaceOrderItems = new List<PlaceOrderItems>();
                var NewPlaceOrderItems = new List<PlaceOrderItems>();
                using (var context = new ApplicationDbContext())
                {
                    var restaurant = context.Restaurants.FirstOrDefault(x => x.Id == rCode);
                    var PlaceOrderdata = context.PlaceOrders.Include(x => x.PlaceOrderItems).Where(x => x.CreatedBy == loginUser.UCode && x.RCode == rCode && x.Table == TCode).ToList();
                    var menudata = context.Menu.Where(x => x.RCode == rCode).ToList();
                    var menuTypes = context.MenuTypes.ToList();
                    //select all order and take placeorderitem from them and add in list
                    PlaceOrderdata = PlaceOrderdata.Select(x =>
                    {
                        PlaceOrderItems.AddRange(x.PlaceOrderItems);
                        return x;
                    }).ToList();
                    foreach (var item in PlaceOrderItems)
                    {
                        var singlemenu = menudata.FirstOrDefault(x => x.MCode == item.MCode && x.RCode == rCode);
                        item.menu = singlemenu;
                        item.menuType = menuTypes.FirstOrDefault(x => x.MTCode == singlemenu.MTCode).MTDesc;
                        item.TotalPrice = (item.price - item.price * Convert.ToInt32(item.Discount) / 100) * item.Quantity;
                        item.Discount = item.Discount;
                        item.PlaceOrder = null;
                        NewPlaceOrderItems.Add(item);
                    }
                    var DiscountedItemCount = PlaceOrderItems.Where(x => x.Discount != "" && x.Discount != null
                    && x.Discount != "0").ToList().Count();
                    DataModel.OrderPlaced = PlaceOrderItems;
                    DataModel.Tax = restaurant.TaxApply;
                    DataModel.Table = TCode;
                    //Bill with all kind of Charges
                    var model = new TotalBillViewModel();
                    model.TotalBill = PlaceOrderItems.Where(x => x.menuType == Constant.Beverage || x.menuType == Constant.NonVeg || x.menuType == Constant.Veg).Select(x => x.TotalPrice).Sum();
                    model.BarTotalBill = PlaceOrderItems.Where(x => x.menuType == Constant.Bar).Select(x => x.TotalPrice).Sum();
                    model.restaurant = restaurant;
                    model.TCode = TCode.ToString();
                    model.RCode = PlaceOrderdata.FirstOrDefault().RCode.ToString();
                    model.isDiscountedItem = DiscountedItemCount > 0 ? true : false;

                    if (PlaceOrderdata.FirstOrDefault(x => x.CouponName != null && x.CouponName != "") != null)
                    {
                        model.CouponCode = PlaceOrderdata.FirstOrDefault().CouponName;
                    }
                    if (removeCoupon == true && model.CouponCode != null && model.CouponCode != "")
                    {
                        PlaceOrderService.Instance.RemoveCouponName(model.CouponCode, loginUser.UCode, loginUser.RCode, loginUser.TCode);
                        model.CouponCode = "";
                    }
                    else if(CouponName != "" && CouponName != null)
                    {
                        model.CouponCode = CouponName;
                    }

                    var BillView = HelperService.Instance.GetBillAmount(model);
                    DataModel.TaxAmount = BillView.TaxAmount;
                    DataModel.ServiceAmount = BillView.ServiceAmount;
                    DataModel.VatAmount = BillView.VatAmount;
                    DataModel.TotalBill = BillView.TotalBill + BillView.BarTotalBill;
                    DataModel.DiscountAmnt = Convert.ToDecimal(BillView.DiscountAmount);
                    DataModel.SubTotalPrice = BillView.SubTotalPrice;
                    DataModel.TaxInc = BillView.TaxInc;
                    DataModel.CouponDiscount = BillView.CouponDiscount;
                    DataModel.isCouponApply = BillView.isCouponApply;
                    DataModel.couponApplyMessage = BillView.CouponMessage;
                    DataModel.CouponDiscountAmount = Convert.ToDecimal(BillView.CouponDiscountAmount);
                    DataModel.CouponAvailable = BillDiscountService.Instance.GetCouponDiscountAvail(rCode.ToString(),TCode);
                    //Bill with all kind of Charges
                    var serializer = new JavaScriptSerializer();
                    var data = serializer.Serialize(DataModel);
                    return Json(data);
                }
            }
            catch (Exception ex)
            {
                return Json(DataModel, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public decimal BillAmountPO(string uCode, string rCode, string tCode, string discount)
        {
            decimal value = 0;
            value = PlaceOrderService.Instance.BillAmountPO(uCode, rCode, tCode, discount);
            return value;
        }
    }
}