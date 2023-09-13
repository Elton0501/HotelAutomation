using DataBase;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class OrderService
    {
        #region singleton
        public static OrderService Instance
        {
            get
            {
                if (instance == null) instance = new OrderService();
                return instance;
            }
        }
        private static OrderService instance { get; set; }

        public OrderService()
        {
        }
        #endregion
        public Orders GetOrderById(string OrderId)
        {
            using (var context = new ApplicationDbContext())
            {
                var order = context.Orders.FirstOrDefault(x => x.OrderId == OrderId);
                return order;
            }
        }
        public int GetTodayAllOrderCount(string rCode)
        {
            using (var context = new ApplicationDbContext())
            {
                int Count = 0;
                if (rCode != "0" && rCode.Length > 0)
                {
                    int RCode = Convert.ToInt32(rCode);
                    DateTime Today = DateTime.Now;
                    int PlaceOrder = context.PlaceOrders.Where(x => x.RCode == RCode && x.CreatedOn.Month == Today.Month && x.CreatedOn.Day == Today.Day).Count();
                    Count = context.Orders.Where(x => x.RCode == RCode && x.CreatedOn.Year == Today.Year && x.CreatedOn.Month == Today.Month && x.CreatedOn.Day == Today.Day).Count() + PlaceOrder;
                }
                return Count;
            }
        }
        public int GetTodayCompleteOrderCount(string rCode)
        {
            using (var context = new ApplicationDbContext())
            {
                int Count = 0;
                if (rCode != "0" && rCode.Length > 0)
                {
                    int RCode = Convert.ToInt32(rCode);
                    DateTime Today = DateTime.Now;
                    Count = context.Orders.Where(x => x.RCode == RCode && x.CreatedOn.Year == Today.Year && x.CreatedOn.Month == Today.Month && x.CreatedOn.Day == Today.Day).Count();
                }
                return Count;
            }
        }
    }
}
