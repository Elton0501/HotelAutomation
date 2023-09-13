using DataBase;
using Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ViewModels;

namespace Services
{
    public class HelperService
    {
        #region singleton
        public static HelperService Instance
        {
            get
            {
                if (instance == null) instance = new HelperService();
                return instance;
            }
        }
        private static HelperService instance { get; set; }

        public HelperService()
        {
        }
        #endregion

        public string Encrypt(string clearText)
        {
            string EncryptionKey = "abc123";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }
        public string Decrypt(string cipherText)
        {
            string EncryptionKey = "abc123";
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }
        public Menu GetRating(int mCode, string rCode)
        {
            using (var context = new ApplicationDbContext())
            {
                int rcode = Convert.ToInt32(rCode);
                return context.Menu.FirstOrDefault(x => x.MCode == mCode && x.RCode == rcode);
            }
        }
        public void AddStarRating(int mCode, string rCode, int starRating)
        {
            using (var context = new ApplicationDbContext())
            {
                int rcode = Convert.ToInt32(rCode);
                var data = context.Menu.FirstOrDefault(x => x.MCode == mCode && x.RCode == rcode);
                var newRating = (Convert.ToInt32(data.Rating != "" && data.Rating != null ? data.Rating : "3") + starRating) / 2;
                data.Rating = newRating.ToString();
                context.Entry(data).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
        }
        public string CreatePassword()
        {
            int length = 6;
            string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789@-";
            Random random = new Random();

            // Select one random character at a time from the string  
            // and create an array of chars  
            char[] chars = new char[length];
            for (int i = 0; i < length; i++)
            {
                chars[i] = validChars[random.Next(0, validChars.Length)];
            }
            return new string(chars);
        }
        public TotalBillViewModel GetBillAmount(TotalBillViewModel model) 
        {
            model.isDiscountApply = false;
            model.isCouponApply = false;
            using (var context = new ApplicationDbContext())
            {
                var restaurant = model.restaurant;
                if (restaurant != null)
                {
                    //if (model.Discount == null || model.Discount == "") { model.Discount = "0"; }
                    //Check Discount or coupon From Discount Table
                    if (model.Discount == "" || model.Discount == null)
                    {
                        model = CheckDiscount(model);
                    }
                    else
                    {
                        model.DiscountAmount = "0";
                        model.isDiscountApply = true;
                        model.DisForFoodandBev = true;
                        model.DisForBar = true;
                    }
                    //Check Discount or coupon End here
                    if (restaurant.TaxApply == true)
                    {
                        //for discount
                        if (Convert.ToInt32(model.DiscountAmount) > 0 && Convert.ToInt32(model.Discount) < 1)
                        {
                            model.Discount = (Convert.ToDecimal(model.DiscountAmount) / Convert.ToDecimal(model.TotalBill + model.BarTotalBill) * 100).ToString();
                        }

                        var FoodDiscount = model.DisForFoodandBev == true ? (model.TotalBill) * Convert.ToDecimal(model.Discount) / 100 : 0;   
                        var BarDiscount = model.DisForBar == true ? (model.BarTotalBill) * Convert.ToDecimal(model.Discount) / 100 : 0;
                        model.Discount = model.Discount;
                        model.DiscountAmount = Convert.ToString(FoodDiscount + BarDiscount);
                        //for Coupon
                        if (model.CouponCode != "" && model.CouponCode != null)
                        {
                            model = CheckCouponDiscount(model);
                            if (Convert.ToDecimal(model.CouponDiscountAmount) >= model.TotalBill + model.BarTotalBill - Convert.ToDecimal(model.DiscountAmount))
                            {
                                model.CouponDiscount = "0";
                                model.CouponDiscountAmount = "0";
                                model.isCouponApply = false;
                                model.CouponMessage = "* This coupon code is not valid";
                                model.CDisForFoodandBev = false;
                                model.CDisForBar = false;
                            }
                        }
                        if (Convert.ToInt32(model.CouponDiscountAmount) > 0 && Convert.ToInt32(model.CouponDiscount) < 1)
                        {
                            model.CouponDiscount = (Convert.ToDecimal(model.CouponDiscountAmount) / Convert.ToDecimal(model.TotalBill + model.BarTotalBill - FoodDiscount - BarDiscount) * 100).ToString();
                        }
                        var FoodCouponDiscount = model.CDisForFoodandBev == true ? (model.TotalBill - FoodDiscount) * Convert.ToDecimal(model.CouponDiscount) / 100 : 0;
                        var BarCouponDiscount = model.CDisForBar == true ? (model.BarTotalBill - BarDiscount) * Convert.ToDecimal(model.CouponDiscount) / 100 : 0;
                        model.BarDiscountAmount = BarCouponDiscount.ToString();
                        model.FoodDiscountAmount = FoodCouponDiscount.ToString();
                        model.CouponDiscount = model.CouponDiscount;
                        model.CouponDiscountAmount = Convert.ToString(FoodCouponDiscount + BarCouponDiscount);

                        //for bill property
                        model.TaxAmount = (model.TotalBill - Convert.ToDecimal(FoodDiscount) - Convert.ToDecimal(FoodCouponDiscount)) * restaurant.Tax / 100;
                        model.VatAmount = (model.BarTotalBill - Convert.ToDecimal(BarDiscount) - Convert.ToDecimal(BarCouponDiscount)) * restaurant.Vat / 100;
                        model.TotalBill = model.TotalBill;
                        model.BarTotalBill = model.BarTotalBill;
                        var totalAmount =  model.TotalBill + model.BarTotalBill - FoodDiscount - BarDiscount - BarCouponDiscount - FoodCouponDiscount;
                        model.ServiceAmount = restaurant.ServiceTax > 0 ? totalAmount * restaurant.ServiceTax / 100 : 0; 
                        model.SubTotalPrice = totalAmount + model.ServiceAmount;
                        model.TaxInc = true;
                    }
                    else
                    {
                        //for discount
                        if (Convert.ToInt32(model.DiscountAmount) > 0 && Convert.ToInt32(model.Discount) < 1)
                        {
                            model.Discount = (Convert.ToDecimal(model.DiscountAmount) / Convert.ToDecimal(model.TotalBill + model.BarTotalBill) * 100).ToString();
                        }
                        var FoodDiscount = model.DisForFoodandBev == true ? (model.TotalBill) * Convert.ToDecimal(model.Discount) / 100: 0;
                        var BarDiscount = model.DisForBar == true ? (model.BarTotalBill) * Convert.ToDecimal(model.Discount) / 100 : 0;
                        model.Discount = model.Discount;
                        model.DiscountAmount = Convert.ToString(FoodDiscount + BarDiscount);

                        //for Coupon
                        if (model.CouponCode != "" && model.CouponCode != null)
                        {
                            model = CheckCouponDiscount(model);
                            if (Convert.ToDecimal(model.CouponDiscountAmount) >= model.TotalBill + model.BarTotalBill - Convert.ToDecimal(model.DiscountAmount))
                            {
                                model.CouponDiscount = "0";
                                model.CouponDiscountAmount = "0";
                                model.isCouponApply = false;
                                model.CouponMessage = "* This coupon code is not valid";
                                model.CDisForFoodandBev = false;
                                model.CDisForBar = false;
                            }
                        }
                        if (Convert.ToInt32(model.CouponDiscountAmount) > 0 && Convert.ToInt32(model.CouponDiscount) < 1)
                        {
                            model.CouponDiscount = (Convert.ToDecimal(model.CouponDiscountAmount) / Convert.ToDecimal(model.TotalBill + model.BarTotalBill - FoodDiscount - BarDiscount) * 100).ToString();
                        }
                        var FoodCouponDiscount = model.CDisForFoodandBev == true ? (model.TotalBill - FoodDiscount) * Convert.ToDecimal(model.CouponDiscount) / 100 : 0;
                        var BarCouponDiscount = model.CDisForBar == true ? (model.BarTotalBill - BarDiscount) * Convert.ToDecimal(model.CouponDiscount) / 100 : 0;
                        model.BarDiscountAmount = BarCouponDiscount.ToString();
                        model.FoodDiscountAmount = FoodCouponDiscount.ToString();
                        model.CouponDiscount = model.CouponDiscount;
                        model.CouponDiscountAmount = Convert.ToString(FoodCouponDiscount + BarCouponDiscount);

                        //for bill property
                        model.TaxAmount = (model.TotalBill - FoodDiscount) * restaurant.Tax / 100;
                        model.VatAmount = (model.BarTotalBill - BarDiscount) * restaurant.Vat / 100;
                        model.ServiceAmount = restaurant.ServiceTax > 0 ? (model.TotalBill + model.BarTotalBill - FoodDiscount - BarDiscount) * restaurant.ServiceTax / 100 : 0;
                        model.SubTotalPrice = model.TotalBill + model.BarTotalBill + model.TaxAmount + model.VatAmount - FoodDiscount - BarDiscount + model.ServiceAmount - Convert.ToDecimal(model.CouponDiscountAmount);
                        model.TaxInc = false;
                    }
                }
            }
                return model;
        }
        public TotalBillViewModel CheckDiscount(TotalBillViewModel model)
        {
            decimal BillAmount = model.TotalBill + model.BarTotalBill;
            string DiscountFor = Constant.DiscountFor.Table;
            if (model.TCode == Constant.TakeAway) 
            { DiscountFor = Constant.TakeAway; }
            else if (model.TCode == Constant.HomeDelivery) 
            { DiscountFor = Constant.HomeDelivery; }

            var GetListDiscount = BillDiscountService.Instance.GetListDiscount(model.RCode, DiscountFor);
            GetListDiscount = model.isDiscountedItem == true ? GetListDiscount.Where(x => x.DiscountOnItem == true).ToList() : GetListDiscount;
            //var selectdata = GetListDiscount.Where(x => x.MinimumAmount <= BillAmount).
            //    OrderByDescending(x => x.MinimumAmount).FirstOrDefault();
            var selectdata = GetListDiscount.Where(x => x.forBar == true && x.forFoodOrBevrage == true ? x.MinimumAmount <= BillAmount 
            : x.forBar == true && x.forFoodOrBevrage == false ? x.MinimumAmount <= model.BarTotalBill 
            : x.forBar != true && x.forFoodOrBevrage == true ? x.MinimumAmount <= model.TotalBill 
            : x.MinimumAmount <= BillAmount).OrderByDescending(x => x.MinimumAmount).FirstOrDefault();
            if (selectdata != null)
            {
                model.DiscountAmount = Convert.ToInt32(selectdata.DiscountAmount) > 0 ? selectdata.DiscountAmount : "0";
                model.Discount = Convert.ToInt32(selectdata.DiscountPercentage) > 0 ? selectdata.DiscountPercentage : "0";
                model.isDiscountApply = true;
                model.DisForFoodandBev = selectdata.forFoodOrBevrage;
                model.DisForBar = selectdata.forBar;
            }
            return model;
        }
        public TotalBillViewModel CheckCouponDiscount(TotalBillViewModel model)
        {
            decimal BillAmount = model.TotalBill + model.BarTotalBill;
            var CurrentUser = UserService.Instance.GetCurrentUserLogin();
            string DiscountFor = Constant.DiscountFor.Table;
            if (model.TCode == Constant.TakeAway)
            { DiscountFor = Constant.TakeAway; }
            else if (model.TCode == Constant.HomeDelivery)
            { DiscountFor = Constant.HomeDelivery; }

            var CouponDiscount = BillDiscountService.Instance.GetCouponDiscount(model.RCode, DiscountFor, model.CouponCode);
            if (CouponDiscount != null)
            {
                if (model.isDiscountedItem == true)
                {
                    CouponDiscount = CouponDiscount.DiscountOnItem == true ? CouponDiscount : null;
                }
                if (model.isDiscountApply == true && CouponDiscount != null)
                {
                    CouponDiscount = CouponDiscount.CouponOnDiscount == true ? CouponDiscount : null;
                }
                if (CouponDiscount != null && CouponDiscount.MinimumAmount > 0)
                {
                    if (CouponDiscount.forFoodOrBevrage == true && CouponDiscount.forBar == true)
                    {
                        model.CouponMessage = BillAmount >= CouponDiscount.MinimumAmount ? null : "*  The Bill amount should be Rs. "
                            + CouponDiscount.MinimumAmount + "to avail this coupon. You have Rs. " + (CouponDiscount.MinimumAmount - BillAmount) + " away from using this coupon.";
                        CouponDiscount = BillAmount >= CouponDiscount.MinimumAmount ? CouponDiscount : null;
                    }
                    else if (CouponDiscount.forFoodOrBevrage == true && CouponDiscount.forBar == false)
                    {
                        model.CouponMessage = model.TotalBill >= CouponDiscount.MinimumAmount ? null : "* The Food amount should be Rs. " + CouponDiscount.MinimumAmount
                            + "to avail this coupon. You have Rs. " + (CouponDiscount.MinimumAmount - model.TotalBill) + " away from using this coupon.";
                        CouponDiscount = model.TotalBill >= CouponDiscount.MinimumAmount ? CouponDiscount : null;
                    }
                    else if (CouponDiscount.forFoodOrBevrage == false && CouponDiscount.forBar == true)
                    {
                        model.CouponMessage = model.BarTotalBill >= CouponDiscount.MinimumAmount ? null : "* The Bar Total should be Rs. " + CouponDiscount.MinimumAmount
                        + "to avail this coupon. You are Rs." + (CouponDiscount.MinimumAmount - model.BarTotalBill) + " away from using this coupon.";
                        CouponDiscount = model.BarTotalBill >= CouponDiscount.MinimumAmount ? CouponDiscount : null;
                    }
                }
                if (CouponDiscount != null && CouponDiscount.isSingleUse == true)
                {
                    var CheckCouponIsUsed = BillDiscountService.Instance.CheckCouponIsUsed(CurrentUser.UCode,
                        CurrentUser.RCode, CouponDiscount.CouponName);
                    CouponDiscount = CheckCouponIsUsed == false ? CouponDiscount : null;
                    model.CouponMessage = CheckCouponIsUsed == true ? "* You have already used this coupon " : null;
                }
                if (CouponDiscount != null && Convert.ToInt32(CouponDiscount.DiscountAmount) > 0)
                {
                    model.CDisForFoodandBev = CouponDiscount.forFoodOrBevrage;
                    model.CDisForBar = CouponDiscount.forBar;
                    model.CouponDiscountAmount = CouponDiscount.DiscountAmount;
                    model.isCouponApply = true;
                    model.CouponMessage = "* You successfully applied the coupon code." + " " + "You saved " + " " + CouponDiscount.DiscountAmount + "Rs on your bill";
                }
                else if (CouponDiscount != null)
                {
                    model.CDisForFoodandBev = CouponDiscount.forFoodOrBevrage;
                    model.CDisForBar = CouponDiscount.forBar;
                    model.CouponDiscount = CouponDiscount.DiscountPercentage;
                    model.isCouponApply = true;
                    model.CouponMessage =  "* You successfully applied the coupon code."+ " " +"You saved "+ " " + CouponDiscount.DiscountPercentage + "% on your bill";
                }
                else if (CouponDiscount == null && model.CouponMessage == null || model.CouponMessage == "")
                {
                    model.CouponMessage = "* This coupon code is not valid";
                }
                //if (model.isCouponApply == true && CouponDiscount.isSingleUse == true)
                //{
                //    AddUserInSingleCoupon(model.CouponCode, model.RCode,CurrentUser.UCode);
                //}
            }
            else
            {
                model.CouponMessage = "* This coupon code is not valid";
            }
            return model;
        }
        public bool AddUserInSingleCoupon(string couponCode, string rCode, string uCode)
        {
            bool result = false;
            using(var context = new ApplicationDbContext())
            {
                var CouponApplied = new CouponApplied();
                CouponApplied.AppliedOn = DateTime.Now.ToString();
                CouponApplied.CouponName = couponCode;
                CouponApplied.UseEmail = uCode;
                CouponApplied.UserNumber = uCode;
                CouponApplied.RCode = rCode;
                context.CouponApplied.Add(CouponApplied);
                context.SaveChanges();
                result = true;
            }
            return result;
        }
        //Elton Encodeing or Decoding for urls and return in numbers
        public string ConvertStringToHex(string input, System.Text.Encoding encoding)
        {
            Byte[] stringBytes = encoding.GetBytes(input);
            StringBuilder sbBytes = new StringBuilder(stringBytes.Length * 2);
            foreach (byte b in stringBytes)
            {
                sbBytes.AppendFormat("{0:X2}", b);
            }
            return sbBytes.ToString();
        }
        public string ConvertHexToString(String hexInput, System.Text.Encoding encoding)
        {
            int numberChars = hexInput.Length;
            byte[] bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hexInput.Substring(i, 2), 16);
            }
            return encoding.GetString(bytes);
        }
    }
}
