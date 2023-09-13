using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class BillViewModel
    {
        public string HotelName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Contact { get; set; }
        public string GSTNo { get; set; }
        public string BillNo { get; set; }
        public string DateOfBill { get; set; }
        public string TimeOfBill { get; set; }
        public object Table { get; set; }
        public List<BillItem> Items { get; set; }
        public string Total { get; set; }
        public List<TaxSummary> taxSummaries { get; set; }
        public string GrandTotal { get; set; }
        public string Comments { get; set; }
        public string ThankYouNote { get; set; }
        public CustomerInfo CustomerInfo { get; set; }
    }
}
