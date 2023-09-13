using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class TotalBillViewModel
    {
        public string RCode { get; set; }
        public string TCode { get; set; }
        //add of bev + food
        public decimal TotalBill { get; set; }
        public decimal FoodTotalBill { get; set; }
        public decimal BevrageTotalBill { get; set; }
        public decimal BarTotalBill { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal VatAmount { get; set; }
        public decimal ServiceAmount { get; set; }
        public bool TaxInc { get; set; }
        public decimal SubTotalPrice { get; set; }
        public Restaurant restaurant { get; set; }

        //Discount and coupon 
        //final discount given by owner or captain to particular user
        public bool isDiscountApply { get; set; }
        public bool isDiscountedItem { get; set; }
        public string Discount { get; set; }
        public bool DisForBar { get; set; }
        public bool DisForFoodandBev { get; set; }
        public string DiscountAmount { get; set; }
        public string FoodDiscountAmount { get; set; }
        public string BarDiscountAmount { get; set; }
        //coupon
        public string CouponDiscount { get; set; }
        public bool CDisForBar { get; set; }
        public bool CDisForFoodandBev { get; set; }
        public string CouponMessage { get; set; }
        public bool isCouponApply { get; set; }
        public string CouponDiscountAmount { get; set; }
        public string CouponCode { get; set; }
    }
}
