using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class PlaceOrderItems
    {
        [Key]
        public long Id { get; set; }
        public string PunchedBy { get; set; }

        public int MCode { get; set; }
        public int Quantity { get; set; }
        public decimal price { get; set; }
        public string Discount { get; set; }
        public long PlaceOrderId { get; set; }
        public PlaceOrder PlaceOrder { get; set; }

        [NotMapped]
        public Menu menu { get; set; }
        [NotMapped]
        public decimal TotalPrice { get; set; }
        [NotMapped]
        public string menuType { get; set; }
        [NotMapped]
        public string menuCat { get; set; }
    }
}
