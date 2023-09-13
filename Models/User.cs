using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class User : Base
    {
        public int Id { get; set; }
        public string MobileNumber { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string DOB { get; set; }
        public bool Varified { get; set; }
        public string HouseNo { get; set; }
        public string Street { get; set; }
        public string LocalArea { get; set; }
        public string LandMark { get; set; }
        public string Pincode { get; set; }
        public Guid RoleId { get; set; }
        [NotMapped]
        public bool isChange { get; set; }
        [NotMapped]
        public int? PaymentType { get; set; }
        [NotMapped]
        public bool isEbill { get; set; }
 
    }
}
