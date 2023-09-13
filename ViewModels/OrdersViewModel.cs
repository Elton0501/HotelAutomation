using System;
namespace ViewModels
{
    public class OrdersViewModel
    {
        public long Id { get; set; }
        public string OrderId { get; set; }
        public Decimal TotalAmount { get; set; }
        public string UserIdentity { get; set; }
        public string Menu { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string  Discount { get; set; }
        public string  PunchedBy { get; set; }
        public string MenuCategory { get; set; }
        public DateTime OrderDate { get; set; }
    }
}
