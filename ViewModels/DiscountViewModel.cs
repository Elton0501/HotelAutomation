using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class DiscountViewModel
    {
        public string Discount { get; set; }
        public bool isAmount { get; set; }
    }
    public class DiscountPerItemViewModel
    {
        public string Discount { get; set; }
        public string result { get; set; }
        public decimal Amount { get; set; }
    }
}
