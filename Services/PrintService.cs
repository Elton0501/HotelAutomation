using DataBase;
using Models;
using Newtonsoft.Json;
using PrinterUtility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Printing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using ViewModels;
using static ViewModels.Constant;

namespace Services
{
    public class PrintService
    {
        #region singleton
        public static PrintService Instance
        {
            get
            {
                if (instance == null) instance = new PrintService();
                return instance;
            }
        }
        private static PrintService instance { get; set; }

        public PrintService()
        {
        }

        public void RemovePrintOrder(int Id)
        {
            using(var context = new ApplicationDbContext())
            {
                var data = context.BillPrintOrders.FirstOrDefault(x=>x.Id == Id);
                if (data != null)
                {
                    context.BillPrintOrders.Remove(data);
                    context.SaveChanges();
                }
            }
        }
        #endregion
        private static List<RootObject> ReportData;
        private static int height;
        private static int itemIndex = 0;
        private static int pageNum = 1;
        private static bool printPage = false;
        private static List<PageSizeSettings> PageSizeSettingsList;
        public object GetPrintDataForApp(BillPrintOrder bill)
        {
            ReportData = null;
            try
            {
              //  KOTViewModel billdata = new KOTViewModel();
                Header header = new Header();
                Settings settings = new Settings();
                CustomerInfo customerInfo = new CustomerInfo();
                List<BillSummary> taxList = new List<BillSummary>();
                var root = new RootObject();
                var rootCom = new List<RootObject>();
              //  TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("Asia/Kolkata");
                DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                //Values for print 
                using (var context = new ApplicationDbContext())
                {
                    var rest = context.Restaurants.FirstOrDefault(x => x.Id == bill.RCode);
                    header.HotelName = rest.Name;
                    header.DateOfBill = indianTime.ToString("dd/MM/yy");
                    header.TimeOfBill = indianTime.ToShortTimeString();
                    root.isEbill = false;
                    if (bill.isBill != true)
                    {
                        long POId = Convert.ToInt64(bill.OrderId);
                        var rOrders = context.Orders.Where(x => x.RCode == bill.RCode && x.BillPayed == true).ToList();
                        int rLastOrderId = rOrders != null ? rOrders.Count() : 0;
                        int currentOrderId = rLastOrderId + 1;
                        string CurrentOID = rest.RPrefix + currentOrderId.ToString();
                        var menu = MenuDetailsService.Instance.GetRestMenus(bill.RCode);
                        var menuType = context.MenuTypes.ToList();
                        var placeorderItem = new List<PlaceOrderItems>();
                        var PlaceOrder = context.PlaceOrders.Include(x => x.PlaceOrderItems).FirstOrDefault(x => x.Id == POId && x.RCode == bill.RCode);
                        if (PlaceOrder == null)
                        {
                            return ReportData;
                        }
                        var placeorderItemList = PlaceOrder.PlaceOrderItems;
                        placeorderItem = placeorderItemList.Select(x =>
                        {
                            x.menuType = menu.FirstOrDefault(y => y.MCode == x.MCode).MTCode.ToString();
                            return x;
                        }).ToList();
                        var Food = placeorderItem.Where(x => x.menuType == menuType.FirstOrDefault(y => y.MTDesc == Constant.Veg).MTCode.ToString() || x.menuType == menuType.FirstOrDefault(y => y.MTDesc == Constant.NonVeg).MTCode.ToString()).ToList();
                        var Beverages = placeorderItem.Where(x => x.menuType == menuType.FirstOrDefault(y => y.MTDesc == Constant.Beverage).MTCode.ToString()).ToList();
                        var Bar = placeorderItem.Where(x => x.menuType == menuType.FirstOrDefault(y => y.MTDesc == Constant.Bar).MTCode.ToString()).ToList();
                        var user = context.User.FirstOrDefault(x => x.MobileNumber == PlaceOrder.CreatedBy || x.Email == PlaceOrder.CreatedBy);
                        //Kitchen
                        if (Food != null && Food.Count() > 0)
                        {
                            var root1 = new RootObject();
                            Header header1 = new Header();
                            Settings settings1 = new Settings();
                            CustomerInfo customerInfo1 = new CustomerInfo();
                            //header
                            header1.Table ="Table: " + PlaceOrder.Table;
                            header1.HotelName = rest.Name;
                            header1.DateOfBill = indianTime.ToString("dd/MM/yy");
                            if (PlaceOrder.Table == Constant.HomeDelivery)
                            { header1.Table = "Home Delivery"; }
                            else if (PlaceOrder.Table == Constant.TakeAway)
                            { header1.Table = "Take Away"; }
                            header1.BillNo = CurrentOID.ToString();
                            header1.FssaiNo = "-";
                            header1.GSTNo = "-";
                            header1.Phone = "-";
                            header1.TimeOfBill = indianTime.ToShortTimeString();
                            header1.Address = "-";
                            //setting
                            settings1.ItemLength = 20;
                            settings1.PageSize = "Size3";
                            settings1.Comments = !string.IsNullOrEmpty(PlaceOrder.Comment) ? PlaceOrder.Comment : "-";
                            //customerInfo
                            if (user != null && !string.IsNullOrEmpty(user.MobileNumber))
                            {
                                customerInfo1.Phone = user.MobileNumber;
                                customerInfo1.Name = "-";
                            }
                            else if(PlaceOrder.CreatedBy != null && PlaceOrder.CreatedBy != "")
                            {
                                customerInfo1.Phone = PlaceOrder.CreatedBy;
                            }
                            //Data fill in the complete model
                            root1.CustomerInfo = customerInfo1;
                            root1.Header = header1;
                            settings1.PrinterName = rest.KitPrinter != null ? rest.KitPrinter : rest.BillPrinter;
                            settings1.PrintType = "Kitchen";
                            settings1.ThankYouNote = "-";
                            //Billitems
                            root1.Items =
                            Food.Select(x => new BillItem
                            {
                                Qty = x.Quantity,
                                MDesc = menu.FirstOrDefault(z => z.MCode == x.MCode).MDesc,
                                IsOld = false,
                                tempToPrint = menu.FirstOrDefault(z => z.MCode == x.MCode).MTCode,
                                Rate = Convert.ToString(MenuDetailsService.Instance.GetRestMenus(bill.RCode).FirstOrDefault(z => z.MCode == x.MCode).Price.ToString("0")),
                                ItemAmt = Convert.ToString(MenuDetailsService.Instance.GetRestMenus(bill.RCode).FirstOrDefault(z => z.MCode == x.MCode).Price * x.Quantity),
                                CustRmks = "-",
                            }).ToList();
                            root1.BarItems= root1.Items;
                            root1.FoodItems = root1.Items;
                            root1.BeverageItems = root1.Items;
                            root1.Settings = settings1;
                            root1.CustomerInfo = customerInfo1;
                            root1.Header = header1;
                            root1.GrandTotal = "-";
                            root1.Total = "-";
                            root1.RoundOff = "-";
                            root1.PaymentType = "-";
                            var tax = new List<BillSummary>(){
                            new BillSummary{key = "-",value = "-",Totalvalue = "-"},
                            }.ToList();
                            taxList.AddRange(tax);
                            root1.BillSummary = taxList;
                            rootCom.Add(root1);
                        }
                        if (Beverages != null && Beverages.Count() > 0)
                        {
                            //header
                            header.Table ="Table: " +  PlaceOrder.Table;
                            if (PlaceOrder.Table == Constant.HomeDelivery)
                            { header.Table = "Home Delivery"; }
                            else if (PlaceOrder.Table == Constant.TakeAway)
                            { header.Table = "Take Away"; }
                            header.BillNo = CurrentOID.ToString();
                            header.FssaiNo = "-";
                            header.GSTNo = "-";
                            header.Phone = "-";
                            header.TimeOfBill = indianTime.ToShortTimeString();
                            header.Address = "-";
                            //setting
                            settings.Comments = !string.IsNullOrEmpty(PlaceOrder.Comment) ? PlaceOrder.Comment : "-";
                            settings.ItemLength = 20;
                            settings.PageSize = "Size3";
                            settings.ThankYouNote = "-";
                            //customerInfo
                            if (user != null && !string.IsNullOrEmpty(user.MobileNumber))
                            {
                                customerInfo.Phone = user.MobileNumber;
                                customerInfo.Name = "-";
                            }
                            else if (PlaceOrder.CreatedBy != null && PlaceOrder.CreatedBy != "")
                            {
                                customerInfo.Phone = PlaceOrder.CreatedBy;
                            }
                            settings.PrinterName = rest.BevPrinter != null ? rest.BevPrinter : rest.BillPrinter;
                            settings.PrintType = "Beverages";
                            root.Items =
                            Beverages.Select(x => new BillItem
                            {
                                Qty = x.Quantity,
                                MDesc = menu.FirstOrDefault(z => z.MCode == x.MCode).MDesc,
                                IsOld = false,
                                CustRmks = "-",
                                tempToPrint = menu.FirstOrDefault(z => z.MCode == x.MCode).MTCode,
                                Rate = Convert.ToString(Math.Round(MenuDetailsService.Instance.GetRestMenus(bill.RCode).FirstOrDefault(z => z.MCode == x.MCode).Price)),
                                ItemAmt = Convert.ToString(MenuDetailsService.Instance.GetRestMenus(bill.RCode).FirstOrDefault(z => z.MCode == x.MCode).Price * x.Quantity),
                            }).ToList();
                            root.BarItems = root.Items;
                            root.FoodItems = root.Items;
                            root.BeverageItems = root.Items;
                            //set For Print
                            root.Settings = settings;
                            root.CustomerInfo = customerInfo;
                            root.Header = header;
                            root.GrandTotal = "-";
                            root.Total = "-";
                            root.RoundOff = "-";
                            root.PaymentType = "-";
                            var tax = new List<BillSummary>(){
                            new BillSummary{key = "-",value = "-",Totalvalue = "-"},
                            }.ToList();
                            taxList.AddRange(tax);
                            root.BillSummary = taxList;
                            rootCom.Add(root);
                        }
                        if (Bar != null && Bar.Count() > 0)
                        {
                            var root2 = new RootObject();
                            Header header2 = new Header();
                            Settings settings2 = new Settings();
                            CustomerInfo customerInfo2 = new CustomerInfo();
                            //header
                            header2.Table ="Table: " + PlaceOrder.Table;
                            header2.HotelName = rest.Name;
                            header2.DateOfBill = indianTime.ToString("dd/MM/yy");
                            if (PlaceOrder.Table == Constant.HomeDelivery)
                            { header2.Table = "Home Delivery"; }
                            else if (PlaceOrder.Table == Constant.TakeAway)
                            { header2.Table = "Take Away"; }
                            header2.BillNo = CurrentOID.ToString();
                            header2.FssaiNo = "-";
                            header2.GSTNo = "-";
                            header2.Phone = "-";
                            header2.TimeOfBill = indianTime.ToShortTimeString();
                            header2.Address = "-";
                            //setting
                            settings2.ItemLength = 20;
                            settings2.PageSize = "Size3";
                            settings2.Comments = !string.IsNullOrEmpty(PlaceOrder.Comment) ? PlaceOrder.Comment : "-";
                            //customerInfo
                            if (user != null && !string.IsNullOrEmpty(user.MobileNumber))
                            {
                                customerInfo2.Phone = user.MobileNumber;
                                customerInfo2.Name = "-";
                            }
                            else if (PlaceOrder.CreatedBy != null && PlaceOrder.CreatedBy != "")
                            {
                                customerInfo2.Phone = PlaceOrder.CreatedBy;
                            }
                            //Data fill in the complete model
                            root2.CustomerInfo = customerInfo2;
                            root2.Header = header2;
                            settings2.PrinterName = rest.BarPrinter != null ? rest.BarPrinter : rest.BillPrinter;
                            settings2.PrintType = "Bar";
                            settings2.ThankYouNote = "-";
                            //Billitems
                            root2.Items =
                            Bar.Select(x => new BillItem
                            {
                                Qty = x.Quantity,
                                MDesc = menu.FirstOrDefault(z => z.MCode == x.MCode).MDesc,
                                IsOld = false,
                                tempToPrint = menu.FirstOrDefault(z => z.MCode == x.MCode).MTCode,
                                Rate = Convert.ToString(MenuDetailsService.Instance.GetRestMenus(bill.RCode).FirstOrDefault(z => z.MCode == x.MCode).Price.ToString("0")),
                                ItemAmt = Convert.ToString(MenuDetailsService.Instance.GetRestMenus(bill.RCode).FirstOrDefault(z => z.MCode == x.MCode).Price * x.Quantity),
                                CustRmks = "-",
                            }).ToList();
                            root2.BarItems = root2.Items;
                            root2.FoodItems = root2.Items;
                            root2.BeverageItems = root2.Items;
                            root2.Settings = settings2;
                            root2.CustomerInfo = customerInfo2;
                            root2.Header = header2;
                            root2.GrandTotal = "-";
                            root2.Total = "-";
                            root2.RoundOff = "-";
                            root2.PaymentType = "-";
                            var tax = new List<BillSummary>(){
                            new BillSummary{key = "-",value = "-",Totalvalue = "-"},
                            }.ToList();
                            taxList.AddRange(tax);
                            root2.BillSummary = taxList;
                            rootCom.Add(root2);
                        }
                    }
                    else
                    {
                        var menu = MenuDetailsService.Instance.GetRestMenus(bill.RCode);
                        var menuType = context.MenuTypes.ToList();
                        //setting
                        settings.PrinterName = rest.BillPrinter;
                        settings.PrintType = "Bill";
                        settings.ThankYouNote = "Thank you, Please visit again.";
                        settings.ItemLength = 20;
                        settings.PageSize = "Size3";
                        //header
                        header.Phone = rest.Mobile;
                        header.Address = rest.Branch + " " + rest.City + " " + rest.Country;
                        if (rest.GSTIN != null) {
                            header.GSTNo = rest.GSTIN;
                        }
                        if (rest.FASSAI != null)
                        {
                            header.FssaiNo = rest.FASSAI;
                        }
                        if (bill.isBill == true && bill.OrderId != string.Empty && bill.OrderId != null && bill.isPlaceOrder == false)
                        {
                            var order = context.Orders.Include(x => x.OrderItem).FirstOrDefault(x => x.RCode == bill.RCode && x.OrderId == bill.OrderId);
                            var menudata = context.Menu.Where(x => x.RCode == bill.RCode).ToList();
                            var menuTypes = context.MenuTypes.ToList();
                            var OrderItems = new List<OrderItem>();
                            OrderItems.AddRange((List<OrderItem>)order.OrderItem.Select(i =>
                            {
                                i.menu = menudata.FirstOrDefault(z => z.MCode == Convert.ToInt32(i.MCode));
                                i.menuType = menuTypes.FirstOrDefault(z => z.MTCode == Convert.ToInt32(i.menu.MTCode)).MTDesc;
                                return i;
                            }).ToList());

                            order.OrderItem = OrderItems;
                            var user = context.User.FirstOrDefault(x => x.MobileNumber == order.CreatedBy || x.Email == order.CreatedBy);
                            //header
                            root.isEbill = order.isEbill;
                            header.Table ="Table: " + order.Table;
                            if (order.Table == Constant.HomeDelivery)
                            { header.Table = "Home Delivery"; }
                            else if (order.Table == Constant.TakeAway)
                            { header.Table = "Take Away"; }
                            header.BillNo = order.OrderId;
                            //customerInfo
                            if (user != null && user.MobileNumber != "" && user.MobileNumber != null)
                            {
                                customerInfo.Phone = user.MobileNumber;
                            }
                            if (user != null && user.Name != "" && user.Name != null)
                            {
                                customerInfo.Name = user.Name.Length < 20 ? user.Name : user.Name.Substring(0, 20);
                                settings.ThankYouNote = "Thank you" + " "+ user.Name + ", Please visit again.";
                            }
                            if (user == null && order.CreatedBy != null && order.CreatedBy != "")
                            {
                                customerInfo.Name = order.CreatedBy;
                            }
                            if (order.Table == Constant.HomeDelivery)
                            { customerInfo.Address1 = order.UserAddress; }
                            //billItems
                            root.Items = order.OrderItem.Select(x => new BillItem
                              {
                                  Qty = x.Quantity,
                                  MDesc = menu.FirstOrDefault(z => z.MCode == x.MCode).MDesc,
                                  IsOld = false,
                                  Rate = Math.Round(x.Price).ToString(),
                                  ItemAmt = Convert.ToString(x.Price * x.Quantity),
                                  ItemAmtInDecimal = x.Price * x.Quantity,
                                  tempToPrint = menu.FirstOrDefault(z => z.MCode == x.MCode).MTCode,
                                  Discount = x.Discount,
                                  MenuTypes = x.menuType,
                                  //Rate = Convert.ToString(MenuDetailsService.Instance.GetRestMenus(bill.RCode).FirstOrDefault(z => z.MCode == x.MCode).Price.ToString("0")),
                                  //ItemAmt = Convert.ToString(MenuDetailsService.Instance.GetRestMenus(bill.RCode).FirstOrDefault(z => z.MCode == x.MCode).Price * x.Quantity),
                                  //tempToPrint = menu.FirstOrDefault(z => z.MCode == x.MCode).MTCode,
                              }).ToList();
                            //Food Items
                            root.FoodItems = order.OrderItem.Where(x=>x.menuType == Constant.Veg || x.menuType == Constant.NonVeg).Select(x => new BillItem
                            {
                                Qty = x.Quantity,
                                MDesc = menu.FirstOrDefault(z => z.MCode == x.MCode).MDesc,
                                IsOld = false,
                                Rate = Math.Round(x.Price).ToString(),
                                ItemAmt = Convert.ToString(x.Price * x.Quantity),
                                ItemAmtInDecimal = x.Price * x.Quantity,
                                tempToPrint = menu.FirstOrDefault(z => z.MCode == x.MCode).MTCode,
                                Discount = x.Discount,
                                MenuTypes = x.menuType,
                                //Rate = Convert.ToString(MenuDetailsService.Instance.GetRestMenus(bill.RCode).FirstOrDefault(z => z.MCode == x.MCode).Price.ToString("0")),
                                //ItemAmt = Convert.ToString(MenuDetailsService.Instance.GetRestMenus(bill.RCode).FirstOrDefault(z => z.MCode == x.MCode).Price * x.Quantity),
                                //tempToPrint = menu.FirstOrDefault(z => z.MCode == x.MCode).MTCode,
                            }).ToList();
                            //Beverage Items
                            root.BeverageItems = order.OrderItem.Where(x => x.menuType == Constant.Beverage).Select(x => new BillItem
                            {
                                Qty = x.Quantity,
                                MDesc = menu.FirstOrDefault(z => z.MCode == x.MCode).MDesc,
                                IsOld = false,
                                Rate = Math.Round(x.Price).ToString(),
                                ItemAmt = Convert.ToString(x.Price * x.Quantity),
                                ItemAmtInDecimal = x.Price * x.Quantity,
                                tempToPrint = menu.FirstOrDefault(z => z.MCode == x.MCode).MTCode,
                                Discount = x.Discount,
                                MenuTypes = x.menuType,
                                //Rate = Convert.ToString(MenuDetailsService.Instance.GetRestMenus(bill.RCode).FirstOrDefault(z => z.MCode == x.MCode).Price.ToString("0")),
                                //ItemAmt = Convert.ToString(MenuDetailsService.Instance.GetRestMenus(bill.RCode).FirstOrDefault(z => z.MCode == x.MCode).Price * x.Quantity),
                                //tempToPrint = menu.FirstOrDefault(z => z.MCode == x.MCode).MTCode,
                            }).ToList();
                            //Bar Items
                            root.BarItems = order.OrderItem.Where(x => x.menuType == Constant.Bar).Select(x => new BillItem
                            {
                                Qty = x.Quantity,
                                MDesc = menu.FirstOrDefault(z => z.MCode == x.MCode).MDesc,
                                IsOld = false,
                                Rate = Math.Round(x.Price).ToString(),
                                ItemAmt = Convert.ToString(x.Price * x.Quantity),
                                ItemAmtInDecimal = x.Price * x.Quantity,
                                tempToPrint = menu.FirstOrDefault(z => z.MCode == x.MCode).MTCode,
                                Discount = x.Discount,
                                MenuTypes = x.menuType,
                                //Rate = Convert.ToString(MenuDetailsService.Instance.GetRestMenus(bill.RCode).FirstOrDefault(z => z.MCode == x.MCode).Price.ToString("0")),
                                //ItemAmt = Convert.ToString(MenuDetailsService.Instance.GetRestMenus(bill.RCode).FirstOrDefault(z => z.MCode == x.MCode).Price * x.Quantity),
                                //tempToPrint = menu.FirstOrDefault(z => z.MCode == x.MCode).MTCode,
                            }).ToList(); 
                          
                            root.GrandTotal = order.TotalAmount.ToString();
                            string symbol = "";
                            string roundOffAmount = "0";
                            if (!string.IsNullOrEmpty(root.GrandTotal))
                            {
                                double valueAmount = (Convert.ToDouble(root.GrandTotal) % 1);
                                symbol = valueAmount > 0.49 ? "+" : "-";
                                roundOffAmount = valueAmount > 0.49 ? Convert.ToString(1.00 - valueAmount) : valueAmount.ToString("N");
                            }
                            root.RoundOff = symbol + " " + roundOffAmount;
                            root.TaxInc = rest.TaxApply;
                            root.Discount = order.Discount;
                            root.DiscountAmnt = order.DiscountAmount.ToString();
                            root.Coupon = order.CouponName;
                            root.CouponDiscount = root.CouponDiscountAmnt;
                            root.CouponDiscountAmnt = root.CouponDiscountAmnt;
                            //root.DiscountAmnt = rest.TaxApply == true ? Convert.ToString(Convert.ToDecimal(root.Items.Select(x => x.ItemAmtInDecimal).Sum()) - order.TotalAmount) :
                            //Convert.ToString(Convert.ToDecimal(root.Items.Select(x => x.ItemAmtInDecimal).Sum()) - order.TotalAmount + order.TaxAmount);
                            //root.Total = rest.TaxApply == true ? Convert.ToString(root.Items.Select(x => x.ItemAmtInDecimal).Sum()) : Convert.ToString((order.TotalAmount - order.TaxAmount) + Convert.ToDecimal(root.DiscountAmnt));
                            root.Total = Convert.ToString(order.BarTotalAmount + order.BevrageTotalAmount + order.FoodTotalAmount);
                            root.PaymentType = order.PaymentType.Value > 0 ? order.PaymentType.Value == (int)PaymentType.Card ? "Card":
                                order.PaymentType.Value == (int)PaymentType.Cash ? "Cash" : order.PaymentType.Value == (int)PaymentType.Upi ? "Online Payment":"-" : "-";
                            root.BarTotal = order.BarTotalAmount.ToString();
                            root.FoodTotal = order.FoodTotalAmount.ToString();
                            root.BeverageTotal = order.BevrageTotalAmount.ToString();
                            var tax = new List<BillSummary>();
                            tax = new List<BillSummary>(){
                            new BillSummary{key = "SGST @" + rest.Tax/2 + "%" + (rest.TaxApply == true ? " (inclusive)" :" (exclusive)"),value = (order.TaxAmount/2).ToString("N"),Totalvalue = (order.TaxAmount + order.VatAmount).ToString()},
                            new BillSummary{key = "CGST @" + rest.Tax/2 + "%" + (rest.TaxApply == true ? " (inclusive)" :" (exclusive)"),value = (order.TaxAmount/2).ToString("N"),Totalvalue = (order.TaxAmount + order.VatAmount).ToString()},
                           }.ToList();
                            if (order.VatAmount.Value > 0)
                            {
                              var VatTax = new BillSummary { key = "VAT @" + rest.Vat + (rest.TaxApply == true ? " (inclusive)" : " (exclusive)"), value = (order.VatAmount.Value).ToString("N"), Totalvalue = (order.TaxAmount + order.VatAmount).ToString() };
                              tax.Add(VatTax);
                            }
                            if (order.ServiceAmount.Value > 0)
                            {
                                var ServiceTax = new BillSummary { key = "Service Tax @" + rest.ServiceTax, value = (order.ServiceAmount.Value).ToString("N"), Totalvalue = order.ServiceAmount.ToString() };
                                tax.Add(ServiceTax);
                            }
                            taxList.AddRange(tax);
                        }
                        else
                        {
                            /*it is used when the order is home delivery and take away, manger or owner can get print the order from 
                            Placeorder data */
                            var PlaceOrderItems = new List<PlaceOrderItems>();
                            var Placeorder = bill.PlaceOrders;
                            var userDetails = Placeorder.FirstOrDefault().CreatedBy;
                            //for (int i = 0; i < Placeorder.Count(); i++)
                            //{
                            //    PlaceOrderItems.AddRange(Placeorder.ElementAtOrDefault(i).PlaceOrderItems);
                            //}
                            int rCode = Convert.ToInt32(Placeorder.FirstOrDefault().RCode);
                            var menudata = context.Menu.Where(x => x.RCode == rCode).ToList();
                            var menuTypes = context.MenuTypes.ToList();
                            Placeorder = Placeorder.Select(x =>
                            {
                                PlaceOrderItems.AddRange(x.PlaceOrderItems.Select(i =>
                                {
                                    i.price = i.price - (i.price * Convert.ToInt32(i.Discount) / 100);
                                    i.menu = menudata.FirstOrDefault(z => z.MCode == Convert.ToInt32(i.MCode));
                                    i.menuType = menuTypes.FirstOrDefault(z => z.MTCode == Convert.ToInt32(i.menu.MTCode)).MTDesc;
                                    i.TotalPrice = i.price * i.Quantity;
                                    return i;
                                }).ToList());
                                return x;
                            }).ToList();
                            var user = context.User.FirstOrDefault(x => x.MobileNumber == userDetails || x.Email == userDetails);
                            //header
                            header.Table ="Table :" + Placeorder.FirstOrDefault().Table;
                            if (Placeorder.FirstOrDefault().Table == Constant.HomeDelivery)
                            { header.Table = "Home Delivery"; }
                            else if (Placeorder.FirstOrDefault().Table == Constant.TakeAway)
                            { header.Table = "Take Away"; }
                            header.BillNo = Placeorder.FirstOrDefault().Id.ToString();
                            //customerInfo
                            if (user != null && user.MobileNumber != "" && user.MobileNumber != null)
                            {
                                customerInfo.Phone = user.MobileNumber;
                            }
                            if (user != null && user.Name != "" && user.Name != null)
                            {
                                customerInfo.Name = user.Name.Length < 13 ? user.Name : user.Name.Substring(0, 13);
                                settings.ThankYouNote = "Thank you" + " " + user.Name + ", Please visit again.";
                            }
                            if (user == null && userDetails != null && userDetails != "")
                            {
                                customerInfo.Name = userDetails;
                            }
                            if (Placeorder.FirstOrDefault().Table == Constant.HomeDelivery)
                            {customerInfo.Address1 = Placeorder.FirstOrDefault(x => x.Address != null) != null 
                                    ? Placeorder.FirstOrDefault(x => x.Address != null).Address : "" ;}
                            //billitems
                            root.Items =
                              PlaceOrderItems.Select(x => new BillItem
                              {
                                  Qty = x.Quantity,
                                  MDesc = menu.FirstOrDefault(z => z.MCode == x.MCode).MDesc,
                                  IsOld = false,
                                 // Rate = Convert.ToString(MenuDetailsService.Instance.GetRestMenus(bill.RCode).FirstOrDefault(z => z.MCode == x.MCode).Price.ToString("0")),
                                 // ItemAmt = Convert.ToString(MenuDetailsService.Instance.GetRestMenus(bill.RCode).FirstOrDefault(z => z.MCode == x.MCode).Price * x.Quantity),
                                  Rate = Convert.ToString(Math.Round(x.price).ToString()),
                                  ItemAmt = Convert.ToString(x.price * x.Quantity),
                                  ItemAmtInDecimal = Convert.ToDecimal(x.price * x.Quantity) ,
                                  tempToPrint = menu.FirstOrDefault(z => z.MCode == x.MCode).MTCode,
                                  Discount = x.Discount,
                                  MenuTypes = x.menuType,
                              }).ToList();
                            //Food Items
                            root.FoodItems = PlaceOrderItems.Where(x => x.menuType == Constant.Veg || x.menuType == Constant.NonVeg).Select(x => new BillItem
                            {
                                Qty = x.Quantity,
                                MDesc = menu.FirstOrDefault(z => z.MCode == x.MCode).MDesc,
                                IsOld = false,
                                // Rate = Convert.ToString(MenuDetailsService.Instance.GetRestMenus(bill.RCode).FirstOrDefault(z => z.MCode == x.MCode).Price.ToString("0")),
                                // ItemAmt = Convert.ToString(MenuDetailsService.Instance.GetRestMenus(bill.RCode).FirstOrDefault(z => z.MCode == x.MCode).Price * x.Quantity),
                                Rate = Convert.ToString(Math.Round(x.price).ToString()),
                                ItemAmt = Convert.ToString(x.price * x.Quantity),
                                ItemAmtInDecimal = Convert.ToDecimal(x.price * x.Quantity),
                                tempToPrint = menu.FirstOrDefault(z => z.MCode == x.MCode).MTCode,
                                Discount = x.Discount,
                                MenuTypes = x.menuType,
                                //Rate = Convert.ToString(MenuDetailsService.Instance.GetRestMenus(bill.RCode).FirstOrDefault(z => z.MCode == x.MCode).Price.ToString("0")),
                                //ItemAmt = Convert.ToString(MenuDetailsService.Instance.GetRestMenus(bill.RCode).FirstOrDefault(z => z.MCode == x.MCode).Price * x.Quantity),
                                //tempToPrint = menu.FirstOrDefault(z => z.MCode == x.MCode).MTCode,
                            }).ToList();
                            //Beverage Items
                            root.BeverageItems = PlaceOrderItems.Where(x => x.menuType == Constant.Beverage).Select(x => new BillItem
                            {
                                Qty = x.Quantity,
                                MDesc = menu.FirstOrDefault(z => z.MCode == x.MCode).MDesc,
                                IsOld = false,
                                // Rate = Convert.ToString(MenuDetailsService.Instance.GetRestMenus(bill.RCode).FirstOrDefault(z => z.MCode == x.MCode).Price.ToString("0")),
                                // ItemAmt = Convert.ToString(MenuDetailsService.Instance.GetRestMenus(bill.RCode).FirstOrDefault(z => z.MCode == x.MCode).Price * x.Quantity),
                                Rate = Convert.ToString(Math.Round(x.price).ToString()),
                                ItemAmt = Convert.ToString(x.price * x.Quantity),
                                ItemAmtInDecimal = Convert.ToDecimal(x.price * x.Quantity),
                                tempToPrint = menu.FirstOrDefault(z => z.MCode == x.MCode).MTCode,
                                Discount = x.Discount,
                                MenuTypes = x.menuType,
                                //Rate = Convert.ToString(MenuDetailsService.Instance.GetRestMenus(bill.RCode).FirstOrDefault(z => z.MCode == x.MCode).Price.ToString("0")),
                                //ItemAmt = Convert.ToString(MenuDetailsService.Instance.GetRestMenus(bill.RCode).FirstOrDefault(z => z.MCode == x.MCode).Price * x.Quantity),
                                //tempToPrint = menu.FirstOrDefault(z => z.MCode == x.MCode).MTCode,
                            }).ToList();
                            //Bar Items
                            root.BarItems = PlaceOrderItems.Where(x => x.menuType == Constant.Bar).Select(x => new BillItem
                            {
                                Qty = x.Quantity,
                                MDesc = menu.FirstOrDefault(z => z.MCode == x.MCode).MDesc,
                                IsOld = false,
                                // Rate = Convert.ToString(MenuDetailsService.Instance.GetRestMenus(bill.RCode).FirstOrDefault(z => z.MCode == x.MCode).Price.ToString("0")),
                                // ItemAmt = Convert.ToString(MenuDetailsService.Instance.GetRestMenus(bill.RCode).FirstOrDefault(z => z.MCode == x.MCode).Price * x.Quantity),
                                Rate = Convert.ToString(Math.Round(x.price).ToString()),
                                ItemAmt = Convert.ToString(x.price * x.Quantity),
                                ItemAmtInDecimal = Convert.ToDecimal(x.price * x.Quantity),
                                tempToPrint = menu.FirstOrDefault(z => z.MCode == x.MCode).MTCode,
                                Discount = x.Discount,
                                MenuTypes = x.menuType,
                                //Rate = Convert.ToString(MenuDetailsService.Instance.GetRestMenus(bill.RCode).FirstOrDefault(z => z.MCode == x.MCode).Price.ToString("0")),
                                //ItemAmt = Convert.ToString(MenuDetailsService.Instance.GetRestMenus(bill.RCode).FirstOrDefault(z => z.MCode == x.MCode).Price * x.Quantity),
                                //tempToPrint = menu.FirstOrDefault(z => z.MCode == x.MCode).MTCode,
                            }).ToList();

                            //Bill with all kind of Charges
                            //TotalBill = Food + Bev;
                            //BarBill is only bar item total
                            var model = new TotalBillViewModel();
                            model.TotalBill = PlaceOrderItems.Where(x => x.menuType == Constant.Beverage || x.menuType == Constant.NonVeg || x.menuType == Constant.Veg).Select(x => x.TotalPrice).Sum();
                            model.FoodTotalBill = PlaceOrderItems.Where(x => x.menuType == Constant.NonVeg || x.menuType == Constant.Veg).Select(x => x.TotalPrice).Sum();
                            model.BevrageTotalBill = PlaceOrderItems.Where(x => x.menuType == Constant.Beverage).Select(x => x.TotalPrice).Sum();
                            model.BarTotalBill = PlaceOrderItems.Where(x => x.menuType == Constant.Bar).Select(x => x.TotalPrice).Sum();
                            model.restaurant = rest;
                            model.TCode = Placeorder.FirstOrDefault().Table.ToString();
                            var BillView = HelperService.Instance.GetBillAmount(model);

                            root.GrandTotal = BillView.SubTotalPrice.ToString();
                            string symbol = "";
                            string roundOffAmount = "0";
                            if (!string.IsNullOrEmpty(root.GrandTotal))
                            {
                                double valueAmount = (Convert.ToDouble(root.GrandTotal) % 1);
                                symbol = valueAmount > 0.49 ? "+" : "-";
                                roundOffAmount = valueAmount > 0.49 ? Convert.ToString(1.00 - valueAmount) : Convert.ToString(valueAmount);
                            }
                            root.RoundOff = symbol + " " + roundOffAmount;
                            root.TaxInc = rest.TaxApply;
                            root.Total = Convert.ToString(BillView.TotalBill + BillView.BarTotalBill);
                            root.Discount = BillView.Discount;
                            root.DiscountAmnt = BillView.DiscountAmount.ToString();
                            root.Coupon = BillView.CouponCode;
                            root.CouponDiscount = BillView.CouponDiscount;
                            root.CouponDiscountAmnt = BillView.CouponDiscountAmount;
                            root.BarTotal = BillView.BarTotalBill.ToString();
                            root.FoodTotal = BillView.FoodTotalBill.ToString();
                            root.BeverageTotal = BillView.BevrageTotalBill.ToString();
                            var tax = new List<BillSummary>();
                            tax = new List<BillSummary>(){
                            new BillSummary{key = "SGST @" + rest.Tax/2 + "%" + (rest.TaxApply == true ? " (inclusive)" : " (exclusive)"),value = (BillView.TaxAmount/2).ToString("N"),Totalvalue = (BillView.TaxAmount + BillView.VatAmount).ToString()},
                            new BillSummary{key = "CGST @" + rest.Tax/2 + "%" + (rest.TaxApply == true ? " (inclusive)" :" (exclusive)"),value = (BillView.TaxAmount/2).ToString("N"),Totalvalue = (BillView.TaxAmount + BillView.VatAmount).ToString()},
                           // new BillSummary{key = "VAT @" + rest.Vat,value = (BillView.VatAmount).ToString("N"),Totalvalue = BillView.VatAmount.ToString()},
                            //new BillSummary{key = "Service Tax @" + rest.ServiceTax ,value = (BillView.ServiceAmount).ToString("N"),Totalvalue = BillView.ServiceAmount.ToString()},
                            }.ToList();
                            if (BillView.VatAmount > 0)
                            {
                                var VatTax = new BillSummary { key = "VAT @" + rest.Vat + (rest.TaxApply == true ? " (inclusive)" : " (exclusive)"), value = (BillView.VatAmount).ToString("N"), Totalvalue = (BillView.TaxAmount + BillView.VatAmount).ToString() };
                                tax.Add(VatTax);
                            }
                            if (BillView.ServiceAmount > 0)
                            {
                                var ServiceTax = new BillSummary { key = "Service Tax @" + rest.ServiceTax, value = (BillView.ServiceAmount).ToString("N"), Totalvalue = (BillView.TaxAmount + BillView.VatAmount).ToString() };
                                tax.Add(ServiceTax);
                            }
                            taxList.AddRange(tax);
                            root.PaymentType = "-";
                            root.isEbill = Placeorder.FirstOrDefault().isEbill;
                        }
                        //set For Print
                        root.Settings = settings;
                        root.CustomerInfo = customerInfo;
                        root.Header = header;
                        root.BillSummary = taxList;
                        //ReportData = root;
                        rootCom.Add(root);
                    }
                    string json = JsonConvert.SerializeObject(rootCom);
                    ReportData = JsonConvert.DeserializeObject<List<RootObject>>(json);
                    PageSizeSettingsList = new List<PageSizeSettings>();
                    PageSizeSettingsList.Add(new PageSizeSettings());
                    return ReportData;
                }
            }
            catch (Exception ex)
            {
                return ReportData;
            }
        }
        public object GetPrintData(BillPrintOrder bill)
        {
            ReportData = null;
            try
            {
                //  KOTViewModel billdata = new KOTViewModel();
                Header header = new Header();
                Settings settings = new Settings();
                CustomerInfo customerInfo = new CustomerInfo();
                List<BillSummary> taxList = new List<BillSummary>();
                var root = new RootObject();
                var rootCom = new List<RootObject>();
                //Values for print 
                using (var context = new ApplicationDbContext())
                {
                    var rest = context.Restaurants.FirstOrDefault(x => x.Id == bill.RCode);
                    header.HotelName = rest.Name;
                    header.DateOfBill = DateTime.Now.ToString("dd/MM/yy") + " " + DateTime.Now.ToShortTimeString();
                    if (bill.isBill != true)
                    {
                        long POId = Convert.ToInt64(bill.OrderId);
                        var rOrders = context.Orders.Where(x => x.RCode == bill.RCode && x.BillPayed == true).ToList();
                        int rLastOrderId = rOrders != null ? rOrders.Count() : 0;
                        int currentOrderId = rLastOrderId + 1;
                        string CurrentOID = rest.RPrefix + currentOrderId.ToString();
                        var menu = MenuDetailsService.Instance.GetRestMenus(bill.RCode);
                        var menuType = context.MenuTypes.ToList();
                        var placeorderItem = new List<PlaceOrderItems>();
                        var PlaceOrder = context.PlaceOrders.Include(x => x.PlaceOrderItems).FirstOrDefault(x => x.Id == POId && x.RCode == bill.RCode);
                        var placeorderItemList = PlaceOrder.PlaceOrderItems;
                        placeorderItem = placeorderItemList.Select(x =>
                        {
                            x.menuType = menu.FirstOrDefault(y => y.MCode == x.MCode).MTCode.ToString();
                            return x;
                        }).ToList();
                        var Food = placeorderItem.Where(x => x.menuType == menuType.FirstOrDefault(y => y.MTDesc == Constant.Veg).MTCode.ToString() || x.menuType == menuType.FirstOrDefault(y => y.MTDesc == Constant.NonVeg).MTCode.ToString()).ToList();
                        var Beverages = placeorderItem.Where(x => x.menuType == menuType.FirstOrDefault(y => y.MTDesc == Constant.Beverage).MTCode.ToString()).ToList();
                        var Bar = placeorderItem.Where(x => x.menuType == menuType.FirstOrDefault(y => y.MTDesc == Constant.Bar).MTCode.ToString()).ToList();
                        var user = context.User.FirstOrDefault(x => x.MobileNumber == PlaceOrder.CreatedBy || x.Email == PlaceOrder.CreatedBy);
                        //Kitchen
                        if (Food != null && Food.Count() > 0)
                        {
                            var root1 = new RootObject();
                            Header header1 = new Header();
                            Settings settings1 = new Settings();
                            CustomerInfo customerInfo1 = new CustomerInfo();
                            //header
                            header1.Table = PlaceOrder.Table;
                            header1.HotelName = rest.Name;
                            header1.DateOfBill = DateTime.Now.ToString("dd/MM/yy") + " " + DateTime.Now.ToShortTimeString();
                            if (PlaceOrder.Table == Constant.HomeDelivery)
                            { header1.Table = "Delivery"; }
                            else if (PlaceOrder.Table == Constant.TakeAway)
                            { header1.Table = "Parcel"; }
                            header1.BillNo = CurrentOID.ToString();
                            //setting
                            settings1.Comments = PlaceOrder.Comment;
                            settings1.ItemLength = 20;
                            settings1.PageSize = "Size3";
                            //customerInfo
                            if (user != null && user.MobileNumber != "" && user.MobileNumber != null)
                            {
                                customerInfo1.Phone = user.MobileNumber;
                            }
                            //Data fill in the complete model
                            root1.CustomerInfo = customerInfo1;
                            root1.Header = header1;
                            settings1.PrinterName = rest.KitPrinter != null ? rest.KitPrinter : rest.BillPrinter;
                            settings1.PrintType = "Kitchen";
                            //Billitems
                            root1.Items =
                            Food.Select(x => new BillItem
                            {
                                Qty = x.Quantity,
                                MDesc = menu.FirstOrDefault(z => z.MCode == x.MCode).MDesc,
                                IsOld = false,
                                tempToPrint = menu.FirstOrDefault(z => z.MCode == x.MCode).MTCode,
                            }).ToList();
                            root1.Settings = settings1;
                            root1.CustomerInfo = customerInfo1;
                            root1.Header = header1;
                            rootCom.Add(root1);
                        }
                        if (Beverages != null && Beverages.Count() > 0)
                        {
                            //header
                            header.Table = PlaceOrder.Table;
                            if (PlaceOrder.Table == Constant.HomeDelivery)
                            { header.Table = "Delivery"; }
                            else if (PlaceOrder.Table == Constant.TakeAway)
                            { header.Table = "Parcel"; }
                            header.BillNo = CurrentOID.ToString();
                            //setting
                            settings.Comments = PlaceOrder.Comment;
                            settings.ItemLength = 20;
                            settings.PageSize = "Size3";
                            //customerInfo
                            if (user != null && user.MobileNumber != "" && user.MobileNumber != null)
                            {
                                customerInfo.Phone = user.MobileNumber;
                            }
                            settings.PrinterName = rest.BevPrinter != null ? rest.BevPrinter : rest.BillPrinter;
                            settings.PrintType = "Beverages";
                            root.Items =
                            Beverages.Select(x => new BillItem
                            {
                                Qty = x.Quantity,
                                MDesc = menu.FirstOrDefault(z => z.MCode == x.MCode).MDesc,
                                IsOld = false,
                                tempToPrint = menu.FirstOrDefault(z => z.MCode == x.MCode).MTCode,
                            }).ToList();
                            //set For Print
                            root.Settings = settings;
                            root.CustomerInfo = customerInfo;
                            root.Header = header;
                            rootCom.Add(root);
                        }
                        if (Bar != null)
                        {

                        }
                    }
                    else
                    {
                        var menu = MenuDetailsService.Instance.GetRestMenus(bill.RCode);
                        var menuType = context.MenuTypes.ToList();
                        //setting
                        settings.PrinterName = rest.BillPrinter;
                        settings.PrintType = "Bill";
                        settings.ThankYouNote = "Thank you, Please visit again.";
                        settings.ItemLength = 20;
                        settings.PageSize = "Size3";
                        //header
                        header.Phone = rest.Mobile;
                        header.Address = rest.Branch + " " + rest.City + " " + rest.Country;
                        if (rest.GSTIN != null)
                        {
                            header.GSTNo = rest.GSTIN;
                        }
                        if (rest.FASSAI != null)
                        {
                            header.FssaiNo = rest.FASSAI;
                        }
                        if (bill.isBill == true && bill.OrderId != string.Empty && bill.OrderId != null)
                        {
                            var order = context.Orders.Include(x => x.OrderItem).FirstOrDefault(x => x.RCode == bill.RCode && x.OrderId == bill.OrderId);
                            var user = context.User.FirstOrDefault(x => x.MobileNumber == order.CreatedBy || x.Email == order.CreatedBy);
                            //header
                            header.Table = order.Table;
                            if (order.Table == Constant.HomeDelivery)
                            { header.Table = "Delivery"; }
                            else if (order.Table == Constant.TakeAway)
                            { header.Table = "Parcel"; }
                            header.BillNo = order.OrderId;
                            //customerInfo
                            if (user != null && user.MobileNumber != "" && user.MobileNumber != null)
                            {
                                customerInfo.Phone = user.MobileNumber;
                            }
                            if (user != null && user.Name != "" && user.Name != null)
                            {
                                customerInfo.Name = user.Name.Length < 20 ? user.Name : user.Name.Substring(0, 20);
                                settings.ThankYouNote = "Thank you," + " " + customerInfo.Name + " " + "Please visit again.";
                            }
                            if (order.Table == Constant.HomeDelivery)
                            { customerInfo.Address1 = order.UserAddress; }
                            //billItems
                            root.Items =
                              order.OrderItem.Select(x => new BillItem
                              {
                                  Qty = x.Quantity,
                                  MDesc = menu.FirstOrDefault(z => z.MCode == x.MCode).MDesc,
                                  IsOld = false,
                                  Rate = Math.Round(x.Price / x.Quantity).ToString(),
                                  ItemAmt = Convert.ToString(x.Price),
                                  ItemAmtInDecimal = x.Price,
                                  tempToPrint = menu.FirstOrDefault(z => z.MCode == x.MCode).MTCode,
                                  Discount = x.Discount
                              }).ToList();
                            root.GrandTotal = order.TotalAmount.ToString();
                            string symbol = "";
                            string roundOffAmount = "0";
                            if (!string.IsNullOrEmpty(root.GrandTotal))
                            {
                                double valueAmount = (Convert.ToDouble(root.GrandTotal) % 1);
                                symbol = valueAmount > 0.49 ? "+" : "-";
                                roundOffAmount = valueAmount > 0.49 ? Convert.ToString(1.00 - valueAmount) : valueAmount.ToString("N");
                            }
                            root.RoundOff = symbol + " " + roundOffAmount;
                            root.TaxInc = rest.TaxApply;
                            root.Discount = order.Discount;
                            root.DiscountAmnt = rest.TaxApply == true ? Convert.ToString(Convert.ToDecimal(root.Items.Select(x=>x.ItemAmtInDecimal).Sum()) - order.TotalAmount) :
                                Convert.ToString(Convert.ToDecimal(root.Items.Select(x => x.ItemAmtInDecimal).Sum()) - order.TotalAmount + order.TaxAmount);
                            root.Total = rest.TaxApply == true ? Convert.ToString(root.Items.Select(x => x.ItemAmtInDecimal).Sum()) : Convert.ToString((order.TotalAmount - order.TaxAmount) + Convert.ToDecimal(root.DiscountAmnt));
                            root.PaymentType = order.PaymentType.Value > 0 ? order.PaymentType.Value == (int)PaymentType.Card ? "Card" :
                                order.PaymentType.Value == (int)PaymentType.Cash ? "Cash" : order.PaymentType.Value == (int)PaymentType.Upi ? "Online Payment" : "-" : "-";
                            var tax = new List<BillSummary>();
                            tax = new List<BillSummary>(){
                            new BillSummary{key = "SGST @2.50%",value = (order.TaxAmount/2).ToString("N"),Totalvalue = order.TaxAmount.ToString()},
                            new BillSummary{key = "CGST @2.50%",value = (order.TaxAmount/2).ToString("N"),Totalvalue = order.TaxAmount.ToString()}
                            }.ToList();
                            taxList.AddRange(tax);
                        }
                        else
                        {
                            /*it is used when the order is home delivery and take away, manger or owner can get print the order from 
                              Placeorder data */
                            var PlaceOrderItems = new List<PlaceOrderItems>();
                            var Placeorder = bill.PlaceOrders;
                            var userDetails = Placeorder.FirstOrDefault().CreatedBy;
                            for (int i = 0; i < Placeorder.Count(); i++)
                            {
                                PlaceOrderItems.AddRange(Placeorder.ElementAtOrDefault(i).PlaceOrderItems);
                            }
                            var user = context.User.FirstOrDefault(x => x.MobileNumber == userDetails || x.Email == userDetails);
                            //header
                            header.Table = Placeorder.FirstOrDefault().Table;
                            if (Placeorder.FirstOrDefault().Table == Constant.HomeDelivery)
                            { header.Table = "Delivery"; }
                            else if (Placeorder.FirstOrDefault().Table == Constant.TakeAway)
                            { header.Table = "Parcel"; }
                            header.BillNo = Placeorder.FirstOrDefault().Id.ToString();
                            //customerInfo
                            if (user != null && user.MobileNumber != "" && user.MobileNumber != null)
                            {
                                customerInfo.Phone = user.MobileNumber;
                            }
                            if (user != null && user.Name != "" && user.Name != null)
                            {
                                customerInfo.Name = user.Name.Length < 13 ? user.Name : user.Name.Substring(0, 13);
                                settings.ThankYouNote = "Thank you," + " " + customerInfo.Name + " " + "Please visit again.";
                            }
                            if (user == null && userDetails != null && userDetails != "")
                            {
                                customerInfo.Name = userDetails;
                            }
                            if (Placeorder.FirstOrDefault().Table == Constant.HomeDelivery)
                            {
                                customerInfo.Address1 = Placeorder.FirstOrDefault(x => x.Address != null) != null
                                       ? Placeorder.FirstOrDefault(x => x.Address != null).Address : "";
                            }
                            //billitems
                            root.Items =
                              PlaceOrderItems.Select(x => new BillItem
                              {
                                  Qty = x.Quantity,
                                  MDesc = menu.FirstOrDefault(z => z.MCode == x.MCode).MDesc,
                                  IsOld = false,
                                  // Rate = Convert.ToString(MenuDetailsService.Instance.GetRestMenus(bill.RCode).FirstOrDefault(z => z.MCode == x.MCode).Price.ToString("0")),
                                  // ItemAmt = Convert.ToString(MenuDetailsService.Instance.GetRestMenus(bill.RCode).FirstOrDefault(z => z.MCode == x.MCode).Price * x.Quantity),
                                  Rate = Convert.ToString(Math.Round(Convert.ToDecimal(x.price) - Convert.ToDecimal(x.price) * Convert.ToInt32(x.Discount) / 100)),
                                  ItemAmt = Convert.ToString((Convert.ToDecimal(x.price) - Convert.ToDecimal(x.price) * Convert.ToInt32(x.Discount) / 100) * x.Quantity),
                                  ItemAmtInDecimal = (Convert.ToDecimal(x.price) - Convert.ToDecimal(x.price) * Convert.ToInt32(x.Discount) / 100) * x.Quantity,
                                  tempToPrint = menu.FirstOrDefault(z => z.MCode == x.MCode).MTCode,
                                  Discount = x.Discount
                              }).ToList();
                            var model = new TotalBillViewModel();
                            model.TotalBill = PlaceOrderItems.Select(x => x.price * x.Quantity).Sum();
                            model.restaurant = rest;
                            var BillView = HelperService.Instance.GetBillAmount(model);
                            root.GrandTotal = BillView.SubTotalPrice.ToString();
                            string symbol = "";
                            string roundOffAmount = "0";
                            if (!string.IsNullOrEmpty(root.GrandTotal))
                            {
                                double valueAmount = (Convert.ToDouble(root.GrandTotal) % 1);
                                symbol = valueAmount > 0.49 ? "+" : "-";
                                roundOffAmount = valueAmount > 0.49 ? Convert.ToString(1.00 - valueAmount) : Convert.ToString(valueAmount);
                            }
                            root.RoundOff = symbol + " " + roundOffAmount;
                            root.TaxInc = rest.TaxApply;
                            root.Total = Convert.ToString(BillView.TotalBill);
                            var tax = new List<BillSummary>();
                            tax = new List<BillSummary>(){
                            new BillSummary{key = "SGST @2.50%",value = (BillView.TaxAmount/2).ToString("N"),Totalvalue = BillView.TaxAmount.ToString()},
                            new BillSummary{key = "CGST @2.50%",value = (BillView.TaxAmount/2).ToString("N"),Totalvalue = BillView.TaxAmount.ToString()}
                            }.ToList();
                            taxList.AddRange(tax);
                            root.PaymentType = "-";
                        }
                        //set For Print
                        root.Settings = settings;
                        root.CustomerInfo = customerInfo;
                        root.Header = header;
                        root.BillSummary = taxList;
                        //ReportData = root;
                        rootCom.Add(root);
                    }
                    string json = JsonConvert.SerializeObject(rootCom);
                    ReportData = JsonConvert.DeserializeObject<List<RootObject>>(json);
                    PageSizeSettingsList = new List<PageSizeSettings>();
                    PageSizeSettingsList.Add(new PageSizeSettings());
                    return ReportData;
                }
            }
            catch (Exception ex)
            {
                return ReportData;
            }
        }
        public BillPrintOrder GetPrintOrders(string rCode)
        {
            using (var context = new ApplicationDbContext())
            {
                int RCode = Convert.ToInt32(rCode);
                var data = context.BillPrintOrders.FirstOrDefault(x => x.RCode == RCode);
                return data;
            }
        }
        public bool saveprintdata(int rCode, string OrderId, bool isBill, bool isPlaceOrder = false)
        {
            bool result = false;
            using (var context = new ApplicationDbContext())
            {
                var printData = new BillPrintOrder();
                printData.RCode = rCode;
                printData.CreatedOn = DateTime.Now;
                printData.isBill = isBill;
                printData.OrderId = OrderId;
                printData.isPlaceOrder = isPlaceOrder;
                context.BillPrintOrders.Add(printData);
                context.SaveChanges();
                result = true;
            }
            return result;
        }
        public bool GetStatusForPrintPO(string rCode)
        {
            bool result = false;
            using (var context = new ApplicationDbContext())
            {
                DateTime Today = DateTime.Now.AddMinutes(-05);
                int RCode = Convert.ToInt32(rCode);
                var data = context.BillPrintOrders.Where(x => x.RCode == RCode).ToList();
                if (data != null)
                {
                    var RemoveData = data.Where(x => x.CreatedOn <= Today || x.CreatedOn.TimeOfDay <= Today.AddMinutes(-05).TimeOfDay).ToList();
                    if (RemoveData.Count != 0 && RemoveData != null)
                    {
                        context.BillPrintOrders.RemoveRange(RemoveData);
                        context.SaveChanges();
                    }
                    var newData = context.BillPrintOrders.ToList();
                    result = newData.Where(x => x.CreatedOn.TimeOfDay >= Today.AddMinutes(-05).TimeOfDay).Count() > 0 ? true : false;
                }
            }
            return result;
        }
    }
}
