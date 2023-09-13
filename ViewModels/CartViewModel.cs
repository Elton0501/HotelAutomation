using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class CartViewModel
    {
        //public List<CartItems> OrderPlaced { get; set; }
        public List<CartItems> CartOrder { get; set; }
        public decimal SubTotalPrice { get; set; }
        public decimal TotalwithoutDisc { get; set; }
        //public decimal TaxAmount { get; set; }
        public bool Tax { get; set; }
        //public decimal TotalBill { get; set; }
        //public string UserAdmin { get; set; }
        public string Table { get; set; }
        public bool IsPlaceOrder { get; set; }

    }
    public class PlaceOrderViewModel
    {
        public List<PlaceOrderItems> OrderPlaced { get; set; }
        public decimal SubTotalPrice { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal VatAmount { get; set; }
        public decimal ServiceAmount { get; set; }
        public bool Tax { get; set; }
        public decimal TotalBill { get; set; }
        public string Table { get; set; }
        public bool TaxInc { get; set; }
        public Decimal DiscountAmnt { get; set; }
        public string CouponDiscount { get; set; }
        public Decimal CouponDiscountAmount { get; set; }
        public bool isCouponApply { get; set; }
        public bool CouponAvailable { get; set; }
        public string couponApplyMessage { get; set; }
    }

    public class CartDataModel
    {
        public string Count { get; set; }
        public Decimal TotalwithoutDisc { get; set; }
        public Decimal TotalAmount { get; set; }
        public bool result { get; set; }
    }
}
