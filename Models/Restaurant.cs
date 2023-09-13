using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Restaurant
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string OwnerName { get; set; }
        [Required]
        public string RPrefix { get; set; }
        [Required]
        public string Branch { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        public string Address { get; set; }
        public string Url { get; set; }
        public string ReviewUrl { get; set; }
        public string GSTIN { get; set; }
        public string FASSAI { get; set; }
        public string TableCount { get; set; }
        [Required]
        public string Mobile { get; set; }
        [Required]
        public string Email { get; set; }
        public string Img { get; set; }
        public string RestImg { get; set; }
        //Include both gst or cgst
        public decimal Tax { get; set; }
        public decimal ServiceTax { get; set; }
        public bool TaxApply { get; set; }
        public decimal Vat { get; set; }
        public bool IsActive { get; set; }
        public bool IsOtpVerification { get; set; }
        public string Key { get; set; }
        public string CaptainKey { get; set; }
        //Package Details of customer
        public int? PDetails { get; set; }
        //Template
        public string Background { get; set; }
        public string Navback { get; set; }
        public string ButtonPrimary { get; set; }
        public string ButtonSecondary { get; set; }
        public string ButtonPrimaryFont { get; set; }
        public string ButtonSecondaryFont { get; set; }
        public string Foodbar { get; set; }
        public string Text { get; set; }
        public string Heading { get; set; }
        public string Label { get; set; }
        public string Bgblur { get; set; }
        //Printer
        public string KitPrinter { get; set; }
        public string BevPrinter { get; set; }
        public string BillPrinter { get; set; }
        public string BarPrinter { get; set; }
        public string PrinterType { get; set; }

        //Events
        public bool YouandMe { get; set; }
        public bool BirthdayParty { get; set; }
        public bool FarewellParty { get; set; }
        public bool FamilyDinner { get; set; }
        public string RestDesc { get; set; }
        public string Youtube { get; set; }
        public string Facebook { get; set; }
        public string Instagram { get; set; }
        public string RestaurantUrl { get; set; }

        ////Discounts
        //public Decimal? DiscountUpto { get; set; }
        //public string Discount { get; set; }
        //public string TADiscount { get; set; }
        //public string HDDiscount { get; set; }
    }
}
