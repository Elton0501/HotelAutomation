using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class EmailOTP
    {
        public int Id { get; set; }
        public int OTP { get; set; }
        public int UId { get; set; }
        public Guid UserDetails { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
