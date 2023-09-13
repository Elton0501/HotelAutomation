using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class SignalRViewModel
    {

    }
    public class SignalROrderModel
    {
        public string CurrentOrderId { get; set; }
        public int TodayOrdersCount { get; set; }
        public int TodayPlaceOrders { get; set; }
        public int MonthlyOrders { get; set; }
        public int YearlyOrders { get; set; }
        public string MonthlyOrdersEarn { get; set; }
        public string YearlyOrdersEarn { get; set; }
    }
    public class SignalRViewOrderModel
    {
        public string CurrentPlaceOrderId { get; set; }
        public int TodayPlaceOrders { get; set; }
    }
}
