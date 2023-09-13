using Models;
using Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using ViewModels;

namespace HotelAutomation.Web.Controllers
{
    public class PrintApiController : ApiController
    {
        CheckoutController checkout = new CheckoutController();
        // GET: api/PrintApi/5
        //[Route("api/GetPrintData")]
        //public object Get(string RCode)
        //{
        //    object value = "Not Found";
        //    var prefix = RCode.Length > 2 ? RCode.Substring(0, 2).ToUpper() : "";
        //    RCode = RCode.Substring(2);
        //    switch (prefix)
        //    {
        //        case Constant.PrintPrefix.Restaurant:
        //            var rest = RestaurantService.Instance.GetRestaurantByID(RCode);
        //            if (rest != null)
        //            {
        //                BillPrintOrder data = PrintService.Instance.GetPrintOrders(RCode);
        //                if (data != null)
        //                {
        //                    value = PrintService.Instance.GetPrintData(data);
        //                    PrintService.Instance.RemovePrintOrder(data.Id);
        //                }
        //            }
        //            break;
        //        case Constant.PrintPrefix.Order:
        //            var order = OrderService.Instance.GetOrderById(RCode);
        //            if (order != null)
        //            {
        //                BillPrintOrder Billorder = new BillPrintOrder();
        //                Billorder.OrderId = order.OrderId;
        //                Billorder.RCode = order.RCode;
        //                Billorder.isBill = true;
        //                value = PrintService.Instance.GetPrintData(Billorder);
        //            }
        //            break;
        //        case Constant.PrintPrefix.PlaceOrder:
        //            var placeOrder = PlaceOrderService.Instance.GetUserAllPlaceOrder(RCode);
        //            if (placeOrder != null)
        //            {
        //                BillPrintOrder Billorder = new BillPrintOrder();
        //                Billorder.RCode = placeOrder.FirstOrDefault().RCode;
        //                Billorder.PlaceOrders = placeOrder;
        //                Billorder.isBill = true;
        //                value = PrintService.Instance.GetPrintData(Billorder);
        //            }
        //            break;
        //    }
        //    return value;
        //}
        //food delivery print is not done, or dummy bill print is not done yet
        [Route("api/GetPrintDataApp")]
        public object GetDataApp(string RCode)
        {
            object value = "Not Found";
            if (!string.IsNullOrEmpty(RCode))
            {
                var rest = RestaurantService.Instance.GetRestaurantByID(RCode);
                if (rest != null)
                {
                    BillPrintOrder data = PrintService.Instance.GetPrintOrders(RCode);
                    if (data != null)
                    {
                        if (data.isPlaceOrder == true)
                        {
                            var placeOrder = PlaceOrderService.Instance.GetUserAllPlaceOrder(data.OrderId);
                            data.PlaceOrders = placeOrder;
                        }
                        value = PrintService.Instance.GetPrintDataForApp(data);
                        PrintService.Instance.RemovePrintOrder(data.Id);
                    }
                }
            }
            return value;
        }
        [Route("api/GetPrinterType")]
        public object GetPrinterType(string RCode)
        {
            object value = null;
            if (!string.IsNullOrEmpty(RCode))
            {
                var data = RestaurantService.Instance.GetRestaurantByID(RCode);
                if (data != null)
                {
                    value = new PrinterTypeModel()
                    {
                        KitPrinter = !string.IsNullOrEmpty(data.KitPrinter) ? data.KitPrinter: data.BillPrinter,
                        BarPrinter = !string.IsNullOrEmpty(data.BarPrinter) ? data.BarPrinter : data.BillPrinter,
                        BevPrinter = !string.IsNullOrEmpty(data.BevPrinter) ? data.BevPrinter : data.BillPrinter,
                        BillPrinter = !string.IsNullOrEmpty(data.BillPrinter) ? data.BillPrinter : "-",
                        PrinterType = !string.IsNullOrEmpty(data.PrinterType) ? data.PrinterType : PrintType.usb.ToString(),
                        RCode = data.Id.ToString(),
                    };
                }
            }
            return value;
        }
    }
}
