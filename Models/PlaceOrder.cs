using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class PlaceOrder : Base
    {
        [Key]
        public long Id { get; set; }
        public string Table { get; set; }
        public int RCode { get; set; }
        public string Address { get; set; }
        public string Comment { get; set; }
        public string CouponName { get; set; }
        public bool isOld { get; set; }
        public bool isServed { get; set; }
        public bool isPrint { get; set; }
        public bool BillPayed { get; set; }
        public List<PlaceOrderItems> PlaceOrderItems { get; set; }
        [NotMapped]
        public decimal TotalPrice { get; set; }
        //elton 
        public bool isEbill { get; set; }
    }
}
