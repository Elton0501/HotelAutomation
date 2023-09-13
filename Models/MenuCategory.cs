using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class MenuCategory : Base
    {
        [Key]
        public int MCCode { get; set; }
        [Required]
        public string MCDesc { get; set; }
        public int RCode { get; set; }
        public bool Veg { get; set; }
        public bool NonVeg { get; set; }
        public bool Bar { get; set; }
        public bool Beverages { get; set; }

        [NotMapped]
        public int? Count { get; set; }
    }
}
