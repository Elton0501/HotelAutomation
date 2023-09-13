using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class Constant
    {
        public static class UserRole
        {
            public const string SuperAdmin = "Super Admin";
            public const string Admin = "Admin";
            public const string User = "User";
        }
        public static class DiscountFor
        {
            public const string HomeDelivery = "Home Delivery";
            public const string Table = "Table";
            public const string TakeAway = "Take Away";
        }
        public static class NotificationConstant
        {
            public const string read = "Read";
            public const string unread = "Unread";
            public const string AddNewUser = "New user scan the barcode from table {0}, user Number is {1}.{2}";
            public const string BirthdayShortMsg = "{0} have Birthday Today.";
        }
        public const string LoginDefaultRCode = "15";
        public const string WebCookie = "SFoodieID";
        public const string TableCode = "TCode";
        public const string RestaurentCode = "RCode";
        public const string Food = "Food";
        public const string Bar = "Bar";
        public const string Veg = "Veg";
        public const string NonVeg = "NonVeg";
        public const string Beverage = "Beverages";
        public const string HomeDelivery = "999";
        public const string TakeAway = "998";
        public const string Events = "997";
        public const string WebHost = ".bfoodie";
        public const int Speciality = 1000;
        public const string SpecialityText = "Restaurant Speciality";
        public static class PrintPrefix
        {
            public const string Restaurant = "R-";
            public const string Order = "O-";
            public const string PlaceOrder = "P-";
        }
    }
    public enum PrinterType
    {
        KitPrinter = 1,
        BevPrinter = 2,
        BarPrinter = 3,
        BillPrinter = 4
    }
    public enum EventType
    {
        YouandMe,
        BirthdayParty = 2,
        FarewellParty = 3,
        FamilyDinner = 4
    }
    public enum RestViewForUserChoiceEnum
    {
        YouandMe = 1,
        getTogather = 2,
        FamilyDinner = 3,
        Farewell = 4
    }
    public enum PaymentType
    {
        Cash = 1,
        Upi = 3,
        Card = 2
    }

    public enum PrintType
    {
        bluetooth,
        usb,
        network
    }

    public static class Unit
    {
        public const string gram = "Gm";
        public const string Kilogram = "Kg";
        public const string mililiter = "Ml";
        public const string liter = "L";
    }
}
