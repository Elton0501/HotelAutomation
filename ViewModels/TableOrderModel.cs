using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class TableOrderModel
    {
        public string DiscountByAdmin { get; set; }
        public List<PlaceOrder> PlaceOrders { get; set; }
        public List<UserDetails> Users { get; set; }
        public Decimal TotalBill { get; set; }
        public string RCode { get; set; }
        public string TCode { get; set; }
        public bool isSingleUser { get; set; }

    }

    public class UserDetails
    {
        public string Name { get; set; }
        public string Contact { get; set; }
    }
}
