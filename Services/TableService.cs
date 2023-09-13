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
    public class TableService
    {
        #region singleton
        public static TableService Instance
        {
            get
            {
                if (instance == null) instance = new TableService();
                return instance;
            }
        }
        private static TableService instance { get; set; }

        public TableService()
        {
        }
        #endregion
        public bool BookTableByQrCode(string UId,string TableId,string RId)
        {
            bool result = false;
            if (UId == "" && UId == null && TableId == "" && TableId == null && RId == "" && RId == null && TableId != Constant.HomeDelivery && TableId != Constant.TakeAway)
            {
                return result;
            }
            using(var context = new ApplicationDbContext())
            {
                var TableData = context.Table.FirstOrDefault(x=>x.RCode == RId && x.TCode == TableId && x.CreatedBy == UId);
                if (TableData == null)
                {
                    var Model = new Table();
                    Model.TCode = TableId;
                    Model.RCode = RId;
                    Model.CreatedBy = UId;
                    Model.CreatedOn = DateTime.Now;
                    context.Table.Add(Model);
                    context.SaveChanges();
                    result = true;
                }
            }
            return result;
        }
        public ResultModel IsUserAdmin(string userIdentity, string rcode,string tcode)
        {
            ResultModel model = new ResultModel();
            model.Result = false;
            using (var context = new ApplicationDbContext())
            {
                var Restdata = context.Table.Where(x=>x.RCode == rcode).ToList();
                var data = Restdata.FirstOrDefault(x=>x.RCode == rcode && x.CreatedBy == userIdentity && x.TCode == tcode);
                if(data != null)
                {
                    model.Result = false;
                    model.Value1 = data.CreatedBy;
                    return model;
                }
                else
                {
                    var UserTable = Restdata.FirstOrDefault(x => x.RCode == rcode && x.TCode == tcode && tcode != Constant.HomeDelivery && tcode != Constant.TakeAway);
                    if (UserTable != null)
                    {
                        model.Result = true;
                        model.Value1 = UserTable.CreatedBy;
                        model.Messsage = "Reserved";
                        return model;
                    }
                }
            }
            return model;
        }
        public bool CheckTableAvailByRCodeTcode(string RCode, string Tcode)
        {
            bool result = false;
            using (var context = new ApplicationDbContext())
            {
                var UserTable = context.Table.FirstOrDefault(x => x.RCode == RCode && x.TCode == Tcode && Tcode != Constant.HomeDelivery && Tcode != Constant.TakeAway);
                if (UserTable != null)
                {
                    result = true;
                }
            }
            return result;
        }
        public IEnumerable<Table> TableUser(string rCode)
        {
            using(var context = new ApplicationDbContext())
            {
                var data = context.Table.Where(x=>x.RCode == rCode).ToList();
                return data;
            }
        }
        public bool RemoveFromTable(int id)
        {
            bool result = false;
            using (var context = new ApplicationDbContext())
            {
                var data = context.Table.FirstOrDefault(x => x.Id == id);
                var user = data.CreatedBy;
                var restCode = Convert.ToInt32(data.RCode);
                var cartdata = context.Cart.Where(x => x.RCode == restCode && x.CreatedBy == user);
                context.Cart.RemoveRange(cartdata);
                context.SaveChanges();
                context.Table.Remove(data);
                context.SaveChanges();
                result = true;
                return result;
            }
        }
        public void DeleteTableUSer(string UserIdentity, int RCode)
        {
            using (var context = new ApplicationDbContext())
            {
                var data = context.Table.FirstOrDefault(x => x.CreatedBy == UserIdentity && x.RCode == RCode.ToString());
                if (data != null)
                {
                    context.Table.Remove(data);
                    context.SaveChanges();
                }
                //var cartdata = context.Cart.Where(x => x.UserIdentity == UserIdentity && x.RCode == RCode).ToList();
                //if (cartdata.Count > 0)
                //{
                //    context.Cart.RemoveRange(cartdata);
                //    context.SaveChanges();
                //}
            }
        }
    }
}
