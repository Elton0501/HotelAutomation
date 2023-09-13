using DataBase;
using Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModels;

namespace Services
{
    public class MenuDetailsService
    {
        #region singleton
        public static MenuDetailsService Instance
        {
            get
            {
                if (instance == null) instance = new MenuDetailsService();
                return instance;
            }
        }
        private static MenuDetailsService instance { get; set; }

        public MenuDetailsService()
        {
        }
        #endregion
        public IEnumerable<Menu> GetRestMenus(int RCode)
        {
            using(var context = new ApplicationDbContext())
            {
                var menutype = context.MenuTypes.ToList();
                var menuCtegory = context.MenuCategory.Where(x => x.RCode == RCode).ToList();
                var data = context.Menu.Where(x => x.RCode == RCode && x.IsActive == true).ToList();
                data = data.Select(x =>
                {
                    x.MenuCategory = menuCtegory.FirstOrDefault(y => y.MCCode == x.MCCode);
                    return x;
                }).ToList();
                data = data != null ? data.Where(x => x.MenuCategory.IsActive == true).ToList() : null;
                return data;
            }
        }
        public string GetRestMenuName(int Id)
        {
            using(var context = new ApplicationDbContext())
            {
                return context.Menu.FirstOrDefault(x => x.MCode == Id).MDesc;
            }
        }
        public IEnumerable<Menu> GetMenusByRCode(IEnumerable<Menu> menu,IEnumerable<MenuType> Menutype, int RCode, bool Vegstatus, bool NonVegstatus, string restType,int? MCCode)
        {
            var menutype = Menutype;
            var data = menu;
            if (MCCode.HasValue && MCCode > 0 && MCCode != Constant.Speciality) { data = data.Where(x => x.MCCode == MCCode).ToList(); } else if(MCCode == Constant.Speciality) { data = data.Where(x => Convert.ToInt32(x.Rating) > 2).ToList(); }
            if (restType == Constant.Food)
            {
                data = data.Where(x => x.MTCode == menutype.Where(y => y.MTDesc == Constant.NonVeg).FirstOrDefault().MTCode
                || x.MTCode == menutype.Where(y => y.MTDesc == Constant.Veg).FirstOrDefault().MTCode).ToList();
                if (Vegstatus == true && NonVegstatus == false)
                {
                    data = data.Where(x => x.MTCode == menutype.Where(y => y.MTDesc == Constant.Veg).FirstOrDefault().MTCode).ToList();
                }
                else if (Vegstatus == false && NonVegstatus == true)
                {
                    data = data.Where(x => x.MTCode == menutype.Where(y => y.MTDesc == Constant.NonVeg).FirstOrDefault().MTCode).ToList();
                }
            }
            else if (restType != null && restType != "")
            {
                data = data.Where(x => x.MTCode == menutype.Where(y => y.MTDesc == restType).FirstOrDefault().MTCode).ToList();
            }
            return data;
        }
        public IEnumerable<MenuType> GetMenuType()
        {
            using (var context = new ApplicationDbContext())
            {
                return context.MenuTypes.ToList();
            }
        }
        public IEnumerable<MenuType> GetActiveMenuType(IEnumerable<Menu> menus)
        {
            using (var context = new ApplicationDbContext())
            {
                var menuData = menus.Where(x => x.IsActive == true).Select(x => x.MCCode).Distinct();
                var data = context.MenuTypes.ToList();
                return data;
            }
        }
        public MenuType GetMenuTypeById(int id)
        {
            using (var context = new ApplicationDbContext())
            {
                return context.MenuTypes.FirstOrDefault(x => x.MTCode == id);
            }
        }
        public IEnumerable<MenuCategory> GetMenuCategoryByRCode(IEnumerable<Menu> menu, IEnumerable<MenuType> menuType, int RCode, bool veg, bool nonveg,string resttype)
        {
            using (var context = new ApplicationDbContext())
            {
                var data = context.MenuCategory.Where(x => x.RCode == RCode).ToList();
                if (veg == true && nonveg == true && resttype == Constant.Food)
                {
                    data = data.Where(x => x.Veg == true || x.NonVeg == true).ToList();
                }
                else if (veg && resttype == Constant.Food) 
                {
                    data = data.Where(x => x.Veg == true).ToList();
                    data = data.Select(x => new MenuCategory
                    {
                        MCCode = x.MCCode,
                        MCDesc = x.MCDesc,RCode = x.RCode,Veg = x.Veg,NonVeg = x.NonVeg,Bar = x.Bar,Beverages = x.Beverages,
                        Count = menu.Where(y => y.MCCode == x.MCCode && y.MTCode == menuType.FirstOrDefault(z => z.MTDesc == Constant.Veg).MTCode).Count(),
                    }).ToList();
                    var emptydata = data.Where(x => x.Count == 0).ToList();
                    for (int i = 0; i < emptydata.Count; i++)
                    {
                        data.RemoveAll(x => x == emptydata[i]);
                    }
                    return data;
                }
                else if (nonveg && resttype == Constant.Food) {data = data.Where(x => x.NonVeg == true).ToList();data = data.Select(x => new MenuCategory{
                        MCCode = x.MCCode,MCDesc = x.MCDesc,RCode = x.RCode,Veg = x.Veg,NonVeg = x.NonVeg,Bar = x.Bar,Beverages = x.Beverages,
                        Count = menu.Where(y => y.MCCode == x.MCCode && y.MTCode == menuType.FirstOrDefault(z=>z.MTDesc == Constant.NonVeg).MTCode).Count(),
                    }).ToList();
                    var emptydata = data.Where(x => x.Count == 0).ToList();
                    for (int i = 0; i < emptydata.Count; i++){data.RemoveAll(x => x == emptydata[i]);}
                    return data;
                }
                else if (resttype == Constant.Bar) {data = data.Where(x => x.Bar == true).ToList();}
                else if (resttype == Constant.Beverage){data = data.Where(x => x.Beverages == true).ToList();}
                data = data.Select(x => new MenuCategory{MCCode = x.MCCode,MCDesc = x.MCDesc,RCode = x.RCode,Veg = x.Veg,NonVeg = x.NonVeg,Bar = x.Bar,Beverages = x.Beverages, Count = menu.Where(y => y.MCCode == x.MCCode).Count(),
                }).ToList();
                var empty = data.Where(x => x.Count == 0).ToList();
                for (int i = 0; i < empty.Count; i++){data.RemoveAll(x => x == empty[i]);}
                return data;
            }
        }
        public IEnumerable<AdminMenuViewModel> GetRestMenusByRCode(int RCode)
        {
            var result = new List<AdminMenuViewModel>();
            using (var context= new ApplicationDbContext())
            {
                var menuCat = context.MenuCategory.Where(x => x.RCode == RCode).ToList();
                var menuType = context.MenuTypes.ToList();
                var data = SuperAdminService.Instance.getMenuList(RCode);
                result = data.Select(x => new AdminMenuViewModel
                {
                    MCCode = x.MCCode,
                    MCDesc = menuCat.FirstOrDefault(z => z.MCCode == x.MCCode).MCDesc,
                    MCode = x.MCode,
                    MDesc = x.MDesc,
                    MenuType = menuType.FirstOrDefault(y => y.MTCode == x.MTCode).MTDesc,
                    Amount = x.Price,
                    CreatedOn = x.CreatedOn.ToString(),
                    Image = x.img,
                    MComments = x.MComment,
                    IsActive = x.IsActive,
                    Discount = x.Discount,
                    TADiscount = x.TADiscount,
                    HDDiscount = x.HDDiscount,
                }).ToList();
            }
            return result;
        }
        public IEnumerable<DropdownMenuViewModel> DropdownMenu(int RCode, int MCCode, string TCode)
        {
            var result = new List<DropdownMenuViewModel>();
            using (var context= new ApplicationDbContext())
            {
                var data = context.Menu.Where(x=>x.RCode == RCode && x.MCCode == MCCode && x.IsActive == true);
                result = data.Select(x => new DropdownMenuViewModel
                {
                    MCode = x.MCode,
                    MDesc = x.MDesc,
                    Discount = TCode == Constant.TakeAway ? x.TADiscount != "0" && x.TADiscount != null ? x.TADiscount : "0" :
                               TCode == Constant.HomeDelivery ? x.HDDiscount != "0" && x.HDDiscount != null ? x.HDDiscount : "0" :
                               x.Discount != "0" && x.Discount != null ? x.Discount : "0",
                }).ToList();
            }
            return result;
        }
        public IEnumerable<MenuCategory> GetRestMenuCategory(int RCode)
        {
            using (var context = new ApplicationDbContext())
            {
                return context.MenuCategory.Where(x => x.RCode == RCode && x.IsActive == true).ToList();
            }
        }
        public string GetDiscountPunchedBy(int rCode, int MCode, string DiscValue, string punchedBy, string TCode)
        {
            using (var context = new ApplicationDbContext())
            {
                var data = context.Menu.FirstOrDefault(x => x.MCode == MCode && x.RCode == rCode);
                int disMenu = 0;
                if (TCode == Constant.HomeDelivery)
                {
                    disMenu = Convert.ToInt32(data.HDDiscount != null && data.HDDiscount != "" ? data.HDDiscount : "0");
                }
                else if(TCode == Constant.TakeAway)
                {
                    disMenu = Convert.ToInt32(data.TADiscount != null && data.TADiscount != "" ? data.TADiscount : "0");
                }
                else
                {
                    disMenu = Convert.ToInt32(data.Discount != null && data.Discount != "" ? data.Discount : "0");
                }
                int disItem = Convert.ToInt32(DiscValue != "" && DiscValue != null ? DiscValue : "0");
                if (disItem > 0)
                {
                    return DiscValue;
                }
                else
                {
                    return disMenu.ToString();
                }
            }
        }
        public string CheckPunchedBy(int rCode,int MCode, string DiscValue, string punchedBy, string TCode)
        {
            using(var context = new ApplicationDbContext())
            {
                var data = context.Menu.FirstOrDefault(x => x.MCode == MCode && x.RCode == rCode);
                int disMenu = 0;
                if (TCode == Constant.HomeDelivery)
                {
                    disMenu = Convert.ToInt32(data.HDDiscount != null && data.HDDiscount != "" ? data.HDDiscount : "0");
                }
                else if (TCode == Constant.TakeAway)
                {
                    disMenu = Convert.ToInt32(data.TADiscount != null && data.TADiscount != "" ? data.TADiscount : "0");
                }
                else
                {
                    disMenu = Convert.ToInt32(data.Discount != null && data.Discount != "" ? data.Discount : "0");
                }
                int disItem = Convert.ToInt32(DiscValue != "" && DiscValue != null ? DiscValue : "0");
                if (disItem == 0 && disMenu > 0 || disItem == disMenu)
                {
                    return "Admin";
                }
                else{
                    return punchedBy;
                }
            }
        }
    }
}
