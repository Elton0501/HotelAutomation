using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class CaptainDetails : Base
    {
        public int Id { get; set; }
        [Required]
        public string RCode { get; set; }
        [Required]
        public string Name { get; set; }
        public string Email { get; set; }
        [Required]
        public string Contact { get; set; }
        public string Address { get; set; }
        [Required]
        public string UniqueCode { get; set; }
    }
}
