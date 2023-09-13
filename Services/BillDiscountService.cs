using DataBase;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModels;

namespace Services
{
    public class BillDiscountService
    {
        #region singleton
        public static BillDiscountService Instance
        {
            get
            {
                if (instance == null) instance = new BillDiscountService();
                return instance;
            }
        }
        private static BillDiscountService instance { get; set; }

        public BillDiscountService()
        {
        }
        #endregion

        public List<BillDiscount> GetListBillDiscount(string rCode)
        {
            using(var context = new ApplicationDbContext())
            {
                return context.BillDiscount.Where(x => x.RCode == rCode).ToList();
            }
        }
        public List<BillDiscount> GetListDiscount(string rCode,string DiscountFor)
        {
            using (var context = new ApplicationDbContext())
            {
                var data = new List<BillDiscount>();
                data = context.BillDiscount.Where(x => x.RCode == rCode).ToList();
                var currentDateTime = DateTime.Now;
                if (DiscountFor == Constant.DiscountFor.TakeAway)
                {
                    data = data.Where(x => x.RCode == rCode
                    && x.CouponName == "" && x.CouponName == null && x.TakeAway == true && x.ExpiredDateTime > currentDateTime).ToList();
                }
                else if (DiscountFor == Constant.DiscountFor.HomeDelivery)
                {
                    data = data.Where(x => x.RCode == rCode
                    && x.CouponName == "" && x.CouponName == null && x.HomeDelivery == true && x.ExpiredDateTime > currentDateTime).ToList();
                }
                else if (DiscountFor == Constant.DiscountFor.Table)
                {
                    data = data.Where(x => x.RCode == rCode
                     && x.Table == true && x.CouponName == "" || x.CouponName == null && x.ExpiredDateTime > currentDateTime).ToList();
                }
                return data;
            }
        }
        public BillDiscount GetCouponDiscount(string rCode,string DiscountFor, string couponCode)
        {
            using (var context = new ApplicationDbContext())
            {
                var data = new BillDiscount();
                var currentDateTime = DateTime.Now;
                if (DiscountFor == Constant.DiscountFor.TakeAway)
                {
                    data = context.BillDiscount.FirstOrDefault(x => x.RCode == rCode
                    && x.CouponName == couponCode && x.TakeAway == true && x.ExpiredDateTime > currentDateTime);
                }
                else if (DiscountFor == Constant.DiscountFor.HomeDelivery)
                {
                    data = context.BillDiscount.FirstOrDefault(x => x.RCode == rCode
                    && x.CouponName == couponCode && x.HomeDelivery == true && x.ExpiredDateTime > currentDateTime);
                }
                else if (DiscountFor == Constant.DiscountFor.Table)
                {
                    data = context.BillDiscount.FirstOrDefault(x => x.RCode == rCode
                     && x.Table == true && x.CouponName == couponCode && x.ExpiredDateTime > currentDateTime);
                }
                return data;
            }
        }
        public BillDiscount GetCoupon(string rCode, string couponCode)
        {
            using (var context = new ApplicationDbContext())
            {
                var data = new BillDiscount();
                if (rCode.Length > 0 && couponCode.Length > 0)
                {
                    data = context.BillDiscount.FirstOrDefault(x => x.RCode == rCode
                    && x.CouponName == couponCode);
                }
                return data;
            }
        }
        public List<BillDiscount> GetCouponList(string rCode)
        {
            using (var context = new ApplicationDbContext())
            {
                var data = new List<BillDiscount>();
                if (rCode.Length > 0)
                {
                    data = context.BillDiscount.Where(x => x.RCode == rCode).ToList();
                }
                return data;
            }
        }
        public bool GetCouponDiscountAvail(string rCode, string DiscountFor)
        {
            using (var context = new ApplicationDbContext())
            {
                var result = false;
                var currentDateTime = DateTime.Now;
                if (DiscountFor == Constant.TakeAway)
                {
                    var data = context.BillDiscount.Where(x => x.RCode == rCode
                    && x.TakeAway == true && x.ExpiredDateTime > currentDateTime).ToList();
                    result = data != null && data.Count > 0 ? true : false;
                }
                else if (DiscountFor == Constant.HomeDelivery)
                {
                    var data = context.BillDiscount.Where(x => x.RCode == rCode
                    && x.HomeDelivery == true && x.ExpiredDateTime > currentDateTime).ToList();
                    result = data != null && data.Count > 0 ? true : false;
                }
                else
                {
                    var data = context.BillDiscount.Where(x => x.RCode == rCode && x.Table == true && x.ExpiredDateTime > currentDateTime).ToList();
                    result = data != null && data.Count > 0 ? true : false;
                }
                return result;
            }
        }
        public bool checkandSaveBillDis(BillDiscount model)
        {
            bool result = false;
            using(var context = new ApplicationDbContext())
            {
                var data = context.BillDiscount.FirstOrDefault(x => x.RCode == model.RCode && x.CouponName != null && x.CouponName != string.Empty
                && x.CouponName == model.CouponName);
                if (data == null)
                {
                    context.BillDiscount.Add(model);
                    context.SaveChanges();
                    result = true;
                    return result;
                }
            }
            return result;
        }

        public bool RemoveBillDiscount(string RCode, int id)
        {
            bool result = false;
            using(var context = new ApplicationDbContext())
            {
                var data = context.BillDiscount.FirstOrDefault(x=>x.Id == id && x.RCode == RCode);
                context.BillDiscount.Remove(data);
                context.SaveChanges();
                result = true;
            }
            return result;
        }

        public bool CheckCouponIsUsed(string uCode, string rCode, string couponName)
        {
            bool result = false;
            using(var context = new ApplicationDbContext())
            {
                var Alldata = context.CouponApplied.ToList();
                var data = context.CouponApplied.FirstOrDefault(x => x.RCode == rCode && x.CouponName == couponName
                && x.UserNumber == uCode || x.UseEmail == uCode);
                if (data != null)
                {
                    result = true;
                }
            }
            return result;
        }
    }
}
