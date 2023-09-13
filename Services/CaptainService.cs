using DataBase;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class CaptainService
    {
        #region singleton
        public static CaptainService Instance
        {
            get
            {
                if (instance == null) instance = new CaptainService();
                return instance;
            }
        }
        private static CaptainService instance { get; set; }

        public CaptainService()
        {
        }
        #endregion
        public List<CaptainDetails> CaptainsList()
        {
            using (var context = new ApplicationDbContext())
            {
                return context.CaptainDetails.ToList();
            }
        }
        public string GetAddCaptain(CaptainDetails details)
        {
            var result = "false";
            using (var context = new ApplicationDbContext())
            {
                context.CaptainDetails.Add(details);
                context.SaveChanges();
                result = "true";
                return result;
            }
        }
        public string GetEditCaptain(CaptainDetails details)
        {
            var result = "false";
            using (var context = new ApplicationDbContext())
            {
                context.Entry(details).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
                result = "true";
                return result;
            }

        }
        public CaptainDetails GetCaptain(int Id)
        {
            using (var context = new ApplicationDbContext())
            {
                var data = context.CaptainDetails.FirstOrDefault(x=>x.Id == Id);
                return data;
            }

        }
        public string GetDeleteCaptain(int Id)
        {
            var result = "false";
            using (var context = new ApplicationDbContext())
            {
                var data = context.CaptainDetails.FirstOrDefault(x=>x.Id == Id);
                context.CaptainDetails.Remove(data);
                context.SaveChanges();
                result = "true";
                return result;
            }

        }

        public bool CheckCId(string uniqueCode, string RCode)
        {
            using(var context= new ApplicationDbContext())
            {
                var data = context.CaptainDetails.FirstOrDefault(x => x.UniqueCode == uniqueCode && x.RCode == RCode);
                return data != null ? true : false;
            }
        }

        public List<CaptainDetails> RestCaptainsList(string rCode)
        {
            using (var context = new ApplicationDbContext())
            {
                var data = context.CaptainDetails.Where(x => x.RCode == rCode).ToList();
                return data;
            }
        }
    }
}
