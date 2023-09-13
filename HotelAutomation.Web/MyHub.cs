using DataBase;
using Microsoft.AspNet.SignalR;
using Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using ViewModels;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace HotelAutomation.Web
{
    public class MyHub : Hub
    {
        public void TotalOrderData(string Rcode, string OrderId)
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<MyHub>();
            //Get TotalNotification  
           var data = new SignalROrderModel();
            data = LoadTotalOrderData(Rcode, OrderId);
            context.Clients.All.broadcaastTotalOrderData(Rcode, data);
        }
        public void PlaceOrderData(string Rcode, string PlaceOrderId)
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<MyHub>();
            //Get TotalNotification  
            var data = new SignalRViewOrderModel();
            data = LoadPlaceOrderData(Rcode, PlaceOrderId);
            context.Clients.All.broadcaastPlaceOrderData(Rcode, data);
        }
        public void NotificationBrodcaast(Notification notification)
        {
            if (notification.Message != null && notification.Message != string.Empty)
            {
                IHubContext context = GlobalHost.ConnectionManager.GetHubContext<MyHub>();
                //Get TotalNotification  
                context.Clients.All.BrodcaastNotificationData(notification.RCode);
            }
        }
        public SignalROrderModel LoadTotalOrderData(string Rcode,string OrderId)
        {
            var data = new SignalROrderModel();
            int rCode = Convert.ToInt32(Rcode);
            data.TodayOrdersCount = 0;
            data.TodayPlaceOrders = 0;
            data.MonthlyOrders = 0;
            data.YearlyOrders = 0;
            data.MonthlyOrdersEarn ="0.00";
            data.YearlyOrdersEarn = "0.00";
            data.CurrentOrderId = OrderId;
            DateTime Today = DateTime.Now;
            using (var context = new ApplicationDbContext())
            {
                var TotalOrders = (from t in context.Orders where t.RCode == rCode select t).ToList();
                var query = (from t in TotalOrders where t.CreatedOn.Year == Today.Year && t.CreatedOn.Month == Today.Month && t.CreatedOn.Day == Today.Day select t).ToList();
                var query1 = (from t in context.PlaceOrders where t.RCode == rCode && t.CreatedOn.Year == Today.Year && t.CreatedOn.Month == Today.Month && t.CreatedOn.Day == Today.Day select t).ToList();
                var query2 = (from t in TotalOrders where t.CreatedOn.Year == Today.Year && t.CreatedOn.Month == Today.Month select t).ToList();
                var query3 = (from t in TotalOrders where t.CreatedOn.Year == Today.Year select t).ToList();
                data.TodayOrdersCount = query.Count;
                data.TodayPlaceOrders = query1.Count;
                data.MonthlyOrders = query2.Count;
                data.YearlyOrders = query3.Count;
                data.MonthlyOrdersEarn = query2.Count > 0 ? query2.Select(x => x.TotalAmount).Sum().ToString("f") : "0.00";
                data.YearlyOrdersEarn = query3.Count > 0 ? query3.Select(x => x.TotalAmount).Sum().ToString("f") : "0.00";
                return data;
            }
        }
        public SignalRViewOrderModel LoadPlaceOrderData(string Rcode, string PlaceOrderId)
        {
            var data = new SignalRViewOrderModel();
            int rCode = Convert.ToInt32(Rcode);
            int total = 0;
            DateTime Today = DateTime.Now;
            using (var context = new ApplicationDbContext())
            {
                var query = (from t in context.PlaceOrders where t.RCode == rCode && t.CreatedOn.Year == Today.Year && t.CreatedOn.Month == Today.Month && t.CreatedOn.Day == Today.Day select t).ToList();
                data.TodayPlaceOrders = query.Count;
                data.CurrentPlaceOrderId = PlaceOrderId;
                return data;
            }
        }
    }
}