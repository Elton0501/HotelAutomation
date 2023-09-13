using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class AdminIndexViewModel
    {
        public List<UserCount> User { get; set; }
        public List<BillDiscount> BillDiscounts { get; set; }
        public List<AdminCartViewModel> AdminCartViewModel { get; set; }
        public List<Orders> Orders { get; set; }
        public decimal MonthlyIncome { get; set; }
        public int MOrdersCount { get; set; }
        public decimal YearlyIncome { get; set; }
        public int YOrdersCount { get; set; }
        public int DineIn { get; set; }
        public int TakeAway { get; set; }
        public int Parcel { get; set; }
        public string OwnerName { get; set; }
        //public string TADiscount { get; set; }
        //public string HDDiscount { get; set; }
    }

    public class AdminCartViewModel
    {
        public long POId { get; set; }
        public string Table { get; set; }
        public string RCode { get; set; }
        public DateTime OrderAt { get; set; }
        public string User { get; set; }
        public string Address { get; set; }
        public List<string> Comments { get; set; }
        public decimal TotalPrice { get; set; }
        public List<PlaceOrderItems> PlaceOrderItems { get; set; }
    }
}
