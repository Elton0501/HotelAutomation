using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Inventory
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string DateTime { get; set; }
        public bool Status { get; set; }

        //stock deatals
        public int InStock { get; set; }
        public int TotalInStock { get; set; }
        public int LowInStock { get; set; }
        [Required]
        public string Unit { get; set; }
    }
}
