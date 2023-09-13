using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class Orders : Base
    {
        [Key]
        public string OrderId { get; set; }
        public string PunchedBy { get; set; }
        public string Table { get; set; } 
        public decimal TotalAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal? VatAmount { get; set; }
        public decimal? ServiceAmount { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? CouponAmount { get; set; }
        public int RCode { get; set; }
        public bool BillPayed { get; set; }
        public List<OrderItem> OrderItem { get; set; }
        public string UserAddress { get; set; }
        public string Discount { get; set; }
        public string CouponDiscount { get; set; }
        public string CouponName { get; set; }

        public int? PaymentType { get; set; }

        public decimal? FoodTotalAmount { get; set; }
        public decimal? BarTotalAmount { get; set; }
        public decimal? BevrageTotalAmount { get; set; }

        //Elton
        public bool isEbill { get; set; }
    }
}
