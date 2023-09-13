using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class PrintObject
    {
        public string Total { get; set; }
        public string GrandTotal { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Phone { get; set; }
        public string key { get; set; }
        public string value { get; set; }
        public List<BillItem> Items { get; set; }
        public string HotelName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string BillNo { get; set; }
        public string DateOfBill { get; set; }
        public object Table { get; set; }
        public string FssaiNo { get; set; }
        public string GSTNo { get; set; }
        public string CustomerRemarksLine1 { get; set; }
        public string CustomerRemarksLine2 { get; set; }
        public string CustomerRemarksLine3 { get; set; }
        public string OrderNote { get; set; }
        public string TimeOfBill { get; set; }
        public Settings Settings { get; set; }

    }
}
