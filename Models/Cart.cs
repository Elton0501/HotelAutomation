using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Cart : Base
    {
        [Key]
        public long Id { get; set; }
        public int RCode { get; set; }
        public string Table { get; set; }
        public List<CartItems> CartItems { get; set; }
        [NotMapped]
        public decimal TotalPrice { get; set; }
    }
}
