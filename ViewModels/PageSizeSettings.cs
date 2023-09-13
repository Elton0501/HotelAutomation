using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class PageSizeSettings
    {
        public PageSizeSettings()
        {
            PageSize = "Size3";
            PageSetting = new PageSettings();
        }
        public string PageSize { get; set; }
        public PageSettings PageSetting { get; set; }
    }
    public class PageSettings
    {
        public PageSettings()
        {
            CashItemFontName = "Segoe UI";
            HeaderCashFontName = "Segoe UI";
            HeaderKitchenFontName = "Segoe UI";
            KitchenItemFontName = "Segoe UI";
            SubHeaderFontName = "Segoe UI";
            CustomerRemarksText = "Remarks: ";
            KitchenCustomerRmkLength = 40;
            ItemPoint = 0;
            QuantityPoint = 90;
            PricePoint = 95;
            AmountPoint = 95;
            ItemWidth = 175;
            RecWidth = 185;
            SummaryPoint = 250;
            TotalPoint = 95;
            WidthSummary = 90;
            KitchenRecWidth = 160;
            KitchenQuantityPoint = 100;
            KitchenItemLength = 22;
            ItemLength = 20;
            HeaderCashFontSize = (float)11;
            CashItemFontSize = (float)7;
            KitchenItemFontSize = (float)8;
            HeaderKitchenFontSize = 11;
            BillNoRowFontSize = 7;
            SubHeaderFontSize = 9;
            XPoint = 2;
            Offset = 3;
            YPoint = 0;
        }

        public string CashItemFontName { get; set; }
        public string HeaderCashFontName { get; set; }
        public string HeaderKitchenFontName { get; set; }
        public string KitchenItemFontName { get; set; }
        public string SubHeaderFontName { get; set; }
        public string CustomerRemarksText { get; set; }
        public int KitchenCustomerRmkLength { get; set; }
        public int ItemPoint { get; set; }
        public int QuantityPoint { get; set; }
        public int PricePoint { get; set; }
        public int AmountPoint { get; set; }
        public int ItemWidth { get; set; }
        public int RecWidth { get; set; }
        public int SummaryPoint { get; set; }
        public int TotalPoint { get; set; }
        public int WidthSummary { get; set; }
        public int KitchenRecWidth { get; set; }
        public int KitchenQuantityPoint { get; set; }
        public int KitchenItemLength { get; set; }
        public int ItemLength { get; set; }
        public float HeaderCashFontSize { get; set; }
        public float CashItemFontSize { get; set; }
        public float KitchenItemFontSize { get; set; }
        public float HeaderKitchenFontSize { get; set; }
        public float BillNoRowFontSize { get; set; }
        public float SubHeaderFontSize { get; set; }
        public int XPoint { get; set; }
        public int Offset { get; set; }
        public int YPoint { get; set; }
    }
    public class CustomerInfo
    {
        public string Address1 { get; set; }
        public string Phone { get; set; }
        public string Name { get; set; }
    }
    public class BillSummary
    {
        public string key { get; set; }
        public string value { get; set; }
        public string Totalvalue { get; set; }
    }
    public class TaxSummary
    {
        public string key { get; set; }
        public string value { get; set; }
    }
    public class Header
    {
        public string HotelName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string BillNo { get; set; }
        public string DateOfBill { get; set; }
        public object Table { get; set; }
        public string FssaiNo { get; set; }
        public string GSTNo { get; set; }
        public string TimeOfBill { get; set; }
    }
    public class BillItem
    {
        public string ItemAmt { get; set; }
        public Decimal ItemAmtInDecimal { get; set; }
        public string MDesc { get; set; }
        public int Qty { get; set; }
        public string Rate { get; set; }
        public string CustRmks { get; set; }
        public bool IsOld { get; set; }
        public int tempToPrint { get; set; }
        public string Discount { get; set; }
        public string MenuTypes { get; set; }
    }

    public class BillItems
    {
        public List<Cart> BillItem { get; set; }
    }
    public class Settings
    {
        public Settings()
        {
            PageSize = "Size3";
        }
        public string PrinterName { get; set; }
        public int ItemLength { get; set; }
        public bool PrintLogo { get; set; }
        public string ThankYouNote { get; set; }
        public string Comments { get; set; }
        public string PrintType { get; set; }
        public string PageSize { get; set; }
    }

    public class RootObject
    {
        public string Total { get; set; }
        public bool TaxInc { get; set; }
        public bool isEbill { get; set; }
        public string GrandTotal { get; set; }
        public string FoodTotal { get; set; }
        public string BeverageTotal { get; set; }
        public string BarTotal { get; set; }
        public string Discount { get; set; }
        public string DiscountAmnt { get; set; }
        public string Coupon { get; set; }
        public string CouponDiscount { get; set; }
        public string CouponDiscountAmnt { get; set; }
        public string RoundOff { get; set; }
        public string PaymentType { get; set; }
        public CustomerInfo CustomerInfo { get; set; }
        public List<BillSummary> BillSummary { get; set; }
        public Header Header { get; set; }
        public List<BillItem> Items { get; set; }
        public List<BillItem> FoodItems { get; set; }
        public List<BillItem> BeverageItems { get; set; }
        public List<BillItem> BarItems { get; set; }
        public Settings Settings { get; set; }
    }
}
