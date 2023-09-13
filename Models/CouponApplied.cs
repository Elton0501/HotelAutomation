using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class CouponApplied
    {
        public int Id { get; set; }
        public string RCode { get; set; }
        public string UserNumber { get; set; }
        public string UseEmail { get; set; }
        public string CouponName { get; set; }
        public string AppliedOn { get; set; }
    }
}
