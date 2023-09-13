using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Package : Base
    {
        public int Id { get; set; }
        [Required]
        public string PName { get; set; }
        public string PDesc { get; set; }
        public decimal Amount { get; set; }
        public int NOT { get; set; }// number of table
        public int NOO { get; set; }// number of Order
        public int MV { get; set; }// month validation
    }
}
