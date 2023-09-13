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
    public class SuperAdminService
    {
        #region singleton
        public static SuperAdminService Instance
        {
            get
            {
                if (instance == null) instance = new SuperAdminService();
                return instance;
            }
        }
        private static SuperAdminService instance { get; set; }

        public SuperAdminService()
        {
        }
        #endregion

        public List<Package> getPackageList()
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var packlist = context.Packages.ToList();
                    return packlist;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public superAdminLogin Login(superAdminLogin model)
        {
            superAdminLogin result = new superAdminLogin();
            result.IsAdmin = false;
            result.IsSuperAdmin = false;
            result.IsCaptain = false;
            using(var context = new ApplicationDbContext())
            {
                var role = context.Roles.ToList();
                if (role == null) { return result; }
                model.Password = HelperService.Instance.Encrypt(model.Password);
                var SARoleID = role.FirstOrDefault(y => y.RoleName == Constant.UserRole.SuperAdmin).RoleId;
                var SuperAdminuser = context.User.FirstOrDefault(x=> x.Email == model.Email && x.Password == model.Password && x.RoleId == SARoleID);
                if (SuperAdminuser != null) { result.IsAdmin =false; result.IsCaptain = false; result.IsSuperAdmin = true; return result; }
                var rest = context.Restaurants.ToList();
                var Adminuser = context.Restaurants.FirstOrDefault(x => x.Email == model.Email && x.Key == model.Password && x.IsActive == true);
                if (Adminuser != null) 
                {
                    result.IsSuperAdmin =false; 
                    result.IsAdmin = true;
                    result.IsCaptain = false;
                    result.RCode = Adminuser.Id;
                    return result; 
                }
                var Captainuser = context.Restaurants.FirstOrDefault(x => x.Email == model.Email && x.CaptainKey == model.Password && x.IsActive == true);
                if (Captainuser != null) 
                {
                    result.IsSuperAdmin =false; 
                    result.IsAdmin = false;
                    result.IsCaptain = true;
                    result.RCode = Captainuser.Id;
                    return result; 
                }
                return result;
            }
        }
        public bool AdminLogin(superAdminLogin model)
        {
            using(var context = new ApplicationDbContext())
            {
                var role = context.Roles.FirstOrDefault(x => x.RoleName == Constant.UserRole.Admin);
                if (role == null) { return false; }
                model.Password = HelperService.Instance.Encrypt(model.Password);
                var user = context.User.FirstOrDefault(x=> x.Email == model.Email && x.Password == model.Password && x.RoleId == role.RoleId);
                if (user != null) { return true; }
                return false;
            }
        }
        public List<MenuType> getMenuTypesList()
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var MenuTypeslist = context.MenuTypes.ToList();
                    return MenuTypeslist;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public List<Restaurant> getRestaurantList()
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var restlist = context.Restaurants.ToList();
                    return restlist;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public List<MenuCategory> getMCatList(int id)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var MCatlist = context.MenuCategory.Where(x => x.RCode == id).ToList();
                    return MCatlist;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public List<Menu> getMenuList(int id)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var Menulist = context.Menu.Where(x => x.RCode == id).ToList();
                    return Menulist;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public List<Orders> getOrderList()
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var Orderlist = context.Orders.ToList();
                    return Orderlist;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public int getMenuCount()
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var Menulist = context.Menu.ToList().Count;
                    return Menulist;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public int getMenuCatCount()
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var MenuCatlist = context.MenuCategory.ToList().Count;
                    return MenuCatlist;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public int getUserCount()
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var Userslist = context.User.ToList().Count;
                    return Userslist;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public int getCartCount()
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var Cartlist = context.Cart.ToList().Count;
                    return Cartlist;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public int getOrderItemsCount()
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var OIlist = context.OrderItem.ToList().Count;
                    return OIlist;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public int getTableCount()
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var Tablelist = context.Table.ToList().Count;
                    return Tablelist;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
