using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class EBillModel
    {
        public Restaurant Restaurant { get; set; }
        public Orders Orders { get; set; }
        public string Total { get; set; }
    }
}
