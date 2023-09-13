using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class BillDiscount : Base
    {
        public int Id { get; set; }
        [Required]
        public string RCode { get; set; }
        [Required]
        public bool HomeDelivery { get; set; }
        [Required]
        public bool TakeAway { get; set; }
        [Required]
        public bool Table { get; set; }
        public string CouponName { get; set; }
        [Required]
        public string DiscountPercentage { get; set; }
        [Required]
        public string DiscountAmount { get; set; }
        public Decimal MinimumAmount { get; set; }
        public bool DiscountOnItem{ get; set; }
        public bool CouponOnDiscount { get; set; }
        public bool isSingleUse { get; set; }
        //new
        public bool forBar { get; set; }
        public bool forFoodOrBevrage { get; set; }
        [Required]
        public DateTime ExpiredDateTime { get; set; }
        //end
        [NotMapped]
        public Restaurant Restaurant { get; set; }
        [NotMapped]
        public bool isDiscount { get; set; }
        [NotMapped]
        public bool isBarAvail { get; set; }
        [NotMapped]
        public bool isFoodAvail { get; set; }
    }
}
