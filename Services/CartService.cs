using DataBase;
using Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using ViewModels;

namespace Services
{
    public class CartService
    {
        #region singleton
        public static CartService Instance
        {
            get
            {
                if (instance == null) instance = new CartService();
                return instance;
            }
        }
        private static CartService instance { get; set; }

        public CartService()
        {
        }
        #endregion
        public CartDataModel AddInCart(int mCode,int rcode, string userIdentity, int TCode)
        {
            using (var context = new ApplicationDbContext())
            {
                CartDataModel model = new CartDataModel();
                model.result = false;
                var existincart = context.Cart.FirstOrDefault(x => x.CreatedBy == userIdentity && x.RCode == rcode && x.Table == TCode.ToString());
                if (existincart == null)
                {
                    var cart = new Cart();
                    cart.RCode = rcode;
                    cart.IsActive = true;
                    cart.Table = TCode.ToString();
                    cart.CreatedBy = userIdentity;
                    cart.CreatedOn = DateTime.Now;
                    context.Cart.Add(cart);
                    context.SaveChanges();
                    existincart = context.Cart.FirstOrDefault(x => x.CreatedBy == userIdentity && x.RCode == rcode && x.Table == TCode.ToString());
                }
                var cartItem = new CartItems();
                //if product alfready exist add the qunatity
                var existincartItem = context.CartItems.FirstOrDefault(x => x.MCode == mCode && x.CartId == existincart.Id);
                if (existincartItem == null)
                {
                    var menu = MenuDetailsService.Instance.GetRestMenus(rcode).FirstOrDefault(x => x.MCode == mCode);
                    cartItem.MCode = mCode;
                    cartItem.Quantity = 1;
                    cartItem.CartId = existincart.Id;
                    cartItem.Discount = TCode.ToString() == Constant.TakeAway ? Convert.ToInt32(menu.TADiscount != null && menu.TADiscount != "" ? menu.TADiscount : "0")
                        : TCode.ToString() == Constant.HomeDelivery ? Convert.ToInt32(menu.HDDiscount != null && menu.HDDiscount != "" ? menu.HDDiscount : "0") :
                         Convert.ToInt32(menu.Discount != null && menu.Discount != "" ? menu.Discount : "0");
                    cartItem.price = MenuDetailsService.Instance.GetRestMenus(rcode).FirstOrDefault(x=>x.MCode == mCode).Price;
                    context.CartItems.Add(cartItem);
                    context.SaveChanges();
                }
                var cartdata = context.CartItems.Where(x => x.CartId == existincart.Id).ToList();
                model.Count = cartdata.Select(x => x.Quantity).Sum().ToString();
                model.TotalAmount = cartdata.Count > 0 ? cartdata.Select(x => (x.price - (x.price * x.Discount.Value / 100)) * x.Quantity).Sum() : 0;
                model.result = true;
                return model;
            }
        }
        public Cart GetUserCart(string UserIdentity, int RCode, string TCode)
        {
            using (var context = new ApplicationDbContext())
            {
                var data = context.Cart.Include(x => x.CartItems).FirstOrDefault(x => x.CreatedBy == UserIdentity && x.RCode == RCode && x.Table == TCode);
                return data != null ? data : null;
            }
        }
        public List<CartItems> GetUserCartItems(string UserIdentity, int? RCode, string TCode)
        {
            using (var context = new ApplicationDbContext())
            {
                var data = context.Cart.Include(x => x.CartItems).FirstOrDefault(x => x.CreatedBy == UserIdentity && x.RCode == RCode.Value && x.Table == TCode);
                return data != null ? data.CartItems : null;
            }
        }
        public List<CartItems> GetUserCartItemsHome(string UserIdentity, int? RCode, string TCode)
        {
            using (var context = new ApplicationDbContext())
            {
                var data = context.Cart.Include(x => x.CartItems).FirstOrDefault(x => x.CreatedBy == UserIdentity && x.RCode == RCode.Value && x.Table == TCode);
                var cartitems = new List<CartItems>();
                if (data != null && data.CartItems.Count > 0)
                {
                    cartitems = data.CartItems;
                    cartitems = cartitems.Select(x => {
                        x.Cart = null;
                        x.TotalPrice = cartitems.Count > 0 ? (Convert.ToDecimal(x.price - (x.price * x.Discount.Value) / 100) * x.Quantity) : 0;
                        return x;
                    }).ToList();
                }
                return cartitems != null ? cartitems : null;
            }
        }
        public CartDataModel RemoveInCart(int ItemId, bool menu,string UserIdentity,int RCode,int TCode)
        {
            CartDataModel model = new CartDataModel();
            model.result = false;
            using (var context = new ApplicationDbContext())
            {
                var cartdata = context.Cart.OrderByDescending(x=>x.Id).Include(x=>x.CartItems).FirstOrDefault(x => x.RCode == RCode && x.CreatedBy == UserIdentity && x.Table == x.Table);
                if (menu == true)
                {
                    var cartItem = cartdata.CartItems.FirstOrDefault(x => x.MCode == ItemId);
                    context.CartItems.Remove(cartItem);
                    context.SaveChanges();
                }
                else
                {
                    var cartItem = context.CartItems.FirstOrDefault(x => x.Id == ItemId); context.CartItems.Remove(cartItem); context.SaveChanges();
                }
                var cartItemdata = cartdata != null ? cartdata.CartItems.ToList() : null;
                model.Count = cartItemdata.Select(x=>x.Quantity).Sum().ToString();
                model.TotalAmount = cartItemdata.Count > 0 ? cartItemdata.Select(x => (x.price - (x.price * x.Discount.Value)/100) * x.Quantity).Sum() : 0;
                model.TotalwithoutDisc = cartItemdata.Count > 0 ? cartItemdata.Select(x => x.price * x.Quantity).Sum() : 0;
                model.result = true;
                return model;
            }
            
        }
    }
}
