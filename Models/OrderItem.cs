using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class OrderItem : Base
    {
        [Key]
        public long Id { get; set; }
        public string OrderId { get; set; }
        public string PunchedBy { get; set; }
        public int MCode { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Discount { get; set; }
        public Orders Orders { get; set; }
        [NotMapped]
        public Menu menu { get; set; }
        [NotMapped]
        public string menuType { get; set; }
        [NotMapped]
        public decimal TotalPrice { get; set; }
    }
}
