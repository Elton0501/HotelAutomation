using DataBase;
using Models;
using System;
using System.Linq;
using System.Data.Entity;
using System.Collections.Generic;
using ViewModels;

namespace Services
{
    public class PlaceOrderService
    {
        #region singleton
        public static PlaceOrderService Instance
        {
            get
            {
                if (instance == null) instance = new PlaceOrderService();
                return instance;
            }
        }
        private static PlaceOrderService instance { get; set; }
        public PlaceOrderService()
        {
        }
        #endregion
        public string PlaceOrderAction(string comment, int rCode, string userIdentity , int TCode)
        {
            string result = "";
            using (var context = new ApplicationDbContext())
            {
                var menu = MenuDetailsService.Instance.GetRestMenus(rCode);
                var CartOrder = context.Cart.Include(x=> x.CartItems).FirstOrDefault(x => x.CreatedBy == userIdentity && x.RCode == rCode && x.Table == TCode.ToString());
                var rest = context.Restaurants.FirstOrDefault(x => x.Id == rCode);
                var PlaceOrder = new PlaceOrder();
                PlaceOrder.Table = TCode.ToString();
                PlaceOrder.Comment = comment;
                PlaceOrder.CreatedBy = userIdentity;
                PlaceOrder.CreatedOn = DateTime.Now;
                PlaceOrder.IsActive = true;
                PlaceOrder.isOld = true;
                PlaceOrder.RCode = rCode;
                PlaceOrder.PlaceOrderItems = CartOrder.CartItems.Select(x => new PlaceOrderItems()
                {
                    MCode = x.MCode,
                    Quantity = x.Quantity,
                    price = x.price,
                    Discount = x.Discount.ToString()
                }).ToList();
                context.PlaceOrders.Add(PlaceOrder);
                context.Cart.Remove(CartOrder);
                context.SaveChanges();
                if (rest.BillPrinter != null && rest.BillPrinter != string.Empty)
                {
                    var saveprintdata = PrintService.Instance.saveprintdata(rCode, Convert.ToString(PlaceOrder.Id), false);
                }
                if (TCode.ToString() == Constant.HomeDelivery || TCode.ToString() == Constant.TakeAway)
                {
                    var UserTable = context.Table.FirstOrDefault(x => x.RCode == rCode.ToString() && x.TCode == TCode.ToString() && TCode.ToString() == Constant.HomeDelivery || TCode.ToString() == Constant.TakeAway && x.CreatedBy == userIdentity);
                    if (UserTable == null)
                    {
                        TableService.Instance.BookTableByQrCode(userIdentity, TCode.ToString(), rCode.ToString());
                    }
                }
                result = PlaceOrder.Id.ToString();
            }
            return result;
        }

        public void AddCouponName(string coupon, string uCode,string rCode,string tCode)
        {
            using (var context = new ApplicationDbContext())
            {
                int RCode = Convert.ToInt32(rCode);
                var data = context.PlaceOrders.Where(x => x.RCode == RCode && x.Table == tCode && x.CreatedBy == uCode).ToList();
                data = data.Select(x =>
                {
                    x.CouponName = coupon;
                    return x;
                }).ToList();
                context.Entry(data.FirstOrDefault()).State = EntityState.Modified;
                context.SaveChanges();
            }
        }
        public void RemoveCouponName(string coupon, string uCode,string rCode,string tCode)
        {
            using (var context = new ApplicationDbContext())
            {
                int RCode = Convert.ToInt32(rCode);
                var data = context.PlaceOrders.FirstOrDefault(x => x.RCode == RCode && x.Table == tCode && x.CreatedBy == uCode && x.CouponName == coupon);
                if (data != null)
                {
                    data.CouponName = null;
                }
                context.Entry(data).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public IEnumerable<PlaceOrder> GetUserAllPlaceOrder(string Id)
        {
            using(var context = new ApplicationDbContext())
            {
                long id = Convert.ToInt64(Id);
                var completeData = context.PlaceOrders.Include(x=>x.PlaceOrderItems).ToList();
                var data = completeData.FirstOrDefault(x => x.Id == id);
                var allData = completeData.Where(x => x.CreatedBy == data.CreatedBy && x.RCode == data.RCode && x.Table == data.Table).ToList();
                return allData;
            }
        }
        public static void DeletePlaceOder(string uCode, int rCode, string tCode)
        {
            using(var context = new ApplicationDbContext())
            {
                var data = context.PlaceOrders.Where(x => x.Table == tCode && x.CreatedBy == uCode && x.RCode == rCode).ToList();
                context.PlaceOrders.RemoveRange(data);
                context.SaveChanges();
            }
        }
        public static void DeletePlaceOderByTableNo(int rCode, string tCode)
        {
            using(var context = new ApplicationDbContext())
            {
                var data = context.PlaceOrders.Where(x => x.Table == tCode && x.RCode == rCode).ToList();
                context.PlaceOrders.RemoveRange(data);
                context.SaveChanges();
            }
        }
        public bool IsPlaceOrder(string uCode, int rCode, string tCode)
        {
            using(var context = new ApplicationDbContext())
            {
                return context.PlaceOrders.FirstOrDefault(x => x.Table == tCode && x.CreatedBy == uCode && x.RCode == rCode) != null ? true : false;
            }
        }
        public decimal BillAmountPO(string uCode, string rCode, string tCode, string Discount)
        {
            using (var context = new ApplicationDbContext())
            {
                int RCode = Convert.ToInt32(rCode);
                decimal tax = 0;
                var restaurant = context.Restaurants.FirstOrDefault(x => x.Id == RCode);
                tax = restaurant.Tax;
                var ItemsList = new List<PlaceOrderItems>();
                var completeData = context.PlaceOrders.Include(x => x.PlaceOrderItems).ToList();
                var allData = completeData.Where(x => x.CreatedBy == uCode && x.RCode == RCode && x.Table == tCode).ToList();
                if (uCode == null || uCode == "" && tCode != null)
                {
                     allData = completeData.Where(x => x.RCode == RCode && x.Table == tCode).ToList();
                }
                var menudata = context.Menu.Where(x => x.RCode == RCode).ToList();
                var menuTypes = context.MenuTypes.ToList();
                foreach (var item in allData)
                {
                    ItemsList.AddRange(item.PlaceOrderItems);
                }
                ItemsList = ItemsList.Select(i =>
                {
                    i.price = i.price - (i.price  * Convert.ToInt32(i.Discount) / 100);
                    i.menu = menudata.FirstOrDefault(z => z.MCode == Convert.ToInt32(i.MCode));
                    i.menuType = menuTypes.FirstOrDefault(z => z.MTCode == Convert.ToInt32(i.menu.MTCode)).MTDesc;
                    i.TotalPrice = i.price * i.Quantity;
                    return i;
                }).ToList();

                var model = new TotalBillViewModel();
                model.TotalBill = ItemsList.Where(x => x.menuType == Constant.Beverage || x.menuType == Constant.NonVeg || x.menuType == Constant.Veg).Select(x => x.TotalPrice).Sum();
                model.FoodTotalBill = ItemsList.Where(x => x.menuType == Constant.NonVeg || x.menuType == Constant.Veg).Select(x => x.TotalPrice).Sum();
                model.BevrageTotalBill = ItemsList.Where(x => x.menuType == Constant.Beverage).Select(x => x.TotalPrice).Sum();
                model.BarTotalBill = ItemsList.Where(x => x.menuType == Constant.Bar).Select(x => x.TotalPrice).Sum();
                model.restaurant = restaurant;
                model.TCode = tCode.ToString();
                if (completeData.FirstOrDefault(x => x.CouponName != null && x.CouponName != "") != null)
                {
                    model.CouponCode = completeData.Where(x => x.CouponName != null && x.CouponName != "").FirstOrDefault().CouponName;
                }
                model.RCode = rCode.ToString();
                model.Discount = Discount;
                var BillView = HelperService.Instance.GetBillAmount(model);
                //decimal TotalPrice = ;
                //decimal Itemsprice = ItemsList.Select(a => a.price).Sum();
                //Itemsprice = tCode == Constant.TakeAway && restaurant.TADiscount != null && Convert.ToInt32(restaurant.TADiscount) > 0 ?
                //    Itemsprice - Itemsprice * Convert.ToDecimal(restaurant.TADiscount) / 100 :
                //    tCode == Constant.HomeDelivery && restaurant.HDDiscount != null && Convert.ToInt32(restaurant.HDDiscount) > 0 ?
                //    Itemsprice - Itemsprice * Convert.ToDecimal(restaurant.HDDiscount) / 100 :
                //    restaurant.Discount != null && Convert.ToInt32(restaurant.Discount) > 0 ?
                //    Itemsprice - Itemsprice * Convert.ToDecimal(restaurant.Discount) / 100 : Itemsprice;
                //decimal TotalPrice = restaurant.TaxApply == true ? Itemsprice :
                //(Itemsprice / 100 * tax + Itemsprice) ;
                return BillView.SubTotalPrice;
            }
        }
    }
}
