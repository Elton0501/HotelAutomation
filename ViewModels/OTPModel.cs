using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class OTPModel
    {
        public string Otp { get; set; }
        public bool Result { get; set; }
        public string Status { get; set; }
        public bool MblNumber { get; set; }
        public string UserIdentity { get; set; }
        public string Details { get; set; }
        public string RCode { get; set; }
        public string TCode { get; set; }
    }
}
