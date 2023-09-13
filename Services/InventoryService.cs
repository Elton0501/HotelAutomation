using DataBase;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class InventoryService
    {
        #region singleton
        public static InventoryService Instance
        {
            get
            {
                if (instance == null) instance = new InventoryService();
                return instance;
            }
        }
        private static InventoryService instance { get; set; }

        public InventoryService()
        {
        }
        #endregion

        public List<Inventory> GetInventoryData()
        {
            try
            {
                using(var context = new ApplicationDbContext())
                {
                    return context.Inventories.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        public Inventory GetInventoryDataById(int Id)
        {
            try
            {
                using(var context = new ApplicationDbContext())
                {
                    return context.Inventories.FirstOrDefault(x=>x.Id == Id);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void InventoryAdd(Inventory inventory)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    context.Inventories.Add(inventory);
                    context.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void InventoryEdit(Inventory inventory)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    context.Entry(inventory).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
