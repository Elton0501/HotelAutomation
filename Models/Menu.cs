using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Menu : Base
    {
        [Key]
        public int MCode { get; set; }
        [Required]
        public int MCCode { get; set; }
        [Required]
        public int MTCode { get; set; }
        [Required]
        public int RCode { get; set; }
        [Required]
        public string MDesc { get; set; }
        public string MComment { get; set; }
        public string Rating { get; set; }
        public string img { get; set; }
        [Required]
        public decimal Price { get; set; }
        public string Discount { get; set; }
        public string TADiscount { get; set; }
        public string HDDiscount { get; set; }
        [NotMapped]
        public MenuCategory MenuCategory { get; set; }
    }
}
