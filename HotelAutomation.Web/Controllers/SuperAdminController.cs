using DataBase;
using Models;
using Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ViewModels;

namespace HotelAutomation.Web.Controllers
{
    [SuperAdminAuthorizationFilterAttribute]
    public class SuperAdminController : Controller
    {
        // GET: SuperAdmin
        public ActionResult Index()
        {
            var model = new SuperAdminViewModel();
            model.RCount = SuperAdminService.Instance.getRestaurantList().Count;
            model.OCount = SuperAdminService.Instance.getOrderList().Count;
            model.OCount = SuperAdminService.Instance.getOrderItemsCount();
            model.CartCount = SuperAdminService.Instance.getCartCount();
            model.MenuCategoryCount = SuperAdminService.Instance.getMenuCatCount();
            model.MenuCount = SuperAdminService.Instance.getMenuCount();
            model.MenuTypeCount = SuperAdminService.Instance.getMenuTypesList().Count;
            model.UsersCount = SuperAdminService.Instance.getUserCount();
            model.UsersCount = UserService.Instance.GetUsers().Count;
            model.TableCount = SuperAdminService.Instance.getTableCount();
            return View(model);
        }
        #region Packages
        [HttpGet]
        public ActionResult PackageDetails()
        {
            var data = SuperAdminService.Instance.getPackageList();
            return View(data);
        }
        public ActionResult _PackageAdd()
        {
            return View();
        }
        [HttpPost]
        public ActionResult _PackageAdd(Package package)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(package);

                }
                package.CreatedOn = DateTime.Now;
                package.IsActive = true;
                using (var context = new ApplicationDbContext())
                {
                    context.Packages.Add(package);
                    context.SaveChanges();
                }
                return RedirectToAction("PackageDetails");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public ActionResult _PackageEdit(int id)
        {
            using (var context = new ApplicationDbContext())
            {
                var data = context.Packages.Where(x => x.Id == id).FirstOrDefault();
                return PartialView(data);
            }
        }
        [HttpPost]
        public ActionResult _PackageEdit(Package package)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(package);

                }
                using (var context = new ApplicationDbContext())
                {
                    context.Entry(package).State = EntityState.Modified;
                    context.SaveChanges();
                }
                return RedirectToAction("PackageDetails");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public ActionResult _PackageDelete(int id)
        {
            try
            {
                var result = "fail";
                using (var context = new ApplicationDbContext())
                {
                    var packdel = context.Packages.FirstOrDefault(x => x.Id == id);
                    context.Packages.Remove(packdel);
                    context.SaveChanges();
                    result = "true";
                }
                return Json(result);
                //return RedirectToAction("PackageDetails", "SuperAdmin");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public ActionResult PackageStatus(int packId, bool status)
        {
            var result = false;
            using (var context = new ApplicationDbContext())
            {
                var data = context.Packages.FirstOrDefault(x => x.Id == packId);
                data.IsActive = status;
                context.Entry(data).State = EntityState.Modified;
                context.SaveChanges();
                result = true;
            }
            return Json(result);
        }
        public ActionResult SinglePackageDetails(int Id)
        {
            var result = new Package();
            using (var context = new ApplicationDbContext())
            {
                result = context.Packages.FirstOrDefault(x => x.Id == Id);
            }
            return View(result);

        }
        #endregion
        #region ClientPackage
        public ActionResult ClientPackageDetails(int id)
        {
            using (var context = new ApplicationDbContext())
            {
                var data = context.ClientPackages.FirstOrDefault(x => x.RCode == id);
                var rdet = context.Restaurants.FirstOrDefault(y => y.Id == id);
                ViewBag.name = rdet.Name;
                ViewBag.branch = rdet.Branch;
                var subid = rdet.PDetails;
                var sdet = context.Packages.FirstOrDefault(z => z.Id == subid);
                ViewBag.sub = sdet.PName;
                return View(data);
            }

        }
        public ActionResult AdvStatus(int advId, bool status)
        {
            var result = false;
            using (var context = new ApplicationDbContext())
            {
                var data = context.ClientPackages.FirstOrDefault(x => x.Id == advId);
                data.IsAPackage = status;
                context.Entry(data).State = EntityState.Modified;
                context.SaveChanges();
                result = true;
            }
            return Json(result);
        }
        #endregion
        #region MenuTypes
        public ActionResult MenuTypes()
        {
            var data = SuperAdminService.Instance.getMenuTypesList();
            return View(data);
        }
        public ActionResult _MenuTypesAdd()
        {
            return PartialView();
        }
        [HttpPost]
        public ActionResult _MenuTypesAdd(MenuType menuType)
        {
            try
            {
                if (menuType.MTDesc == null)
                {
                    return View(menuType);
                }
                menuType.CreatedOn = DateTime.Now;
                menuType.IsActive = true;
                using (var context = new ApplicationDbContext())
                {
                    context.MenuTypes.Add(menuType);
                    context.SaveChanges();
                }
                return RedirectToAction("MenuTypes");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public ActionResult MenuTypeStatus(int menutId, bool status)
        {
            var result = false;
            using (var context = new ApplicationDbContext())
            {
                var data = context.MenuTypes.FirstOrDefault(x => x.MTCode == menutId);
                data.IsActive = status;
                context.Entry(data).State = EntityState.Modified;
                context.SaveChanges();
                result = true;
            }
            return Json(result);
        }
        public ActionResult _MenuTypesEdit(int id)
        {
            using (var context = new ApplicationDbContext())
            {
                var data = context.MenuTypes.Where(x => x.MTCode == id).FirstOrDefault();
                return PartialView(data);
            }
        }
        [HttpPost]
        public ActionResult _MenuTypesEdit(MenuType menuType)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(menuType);

                }
                using (var context = new ApplicationDbContext())
                {
                    context.Entry(menuType).State = EntityState.Modified;
                    context.SaveChanges();
                }
                return RedirectToAction("MenuTypes");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion
        #region Restaurants
        [HttpGet]
        public ActionResult Restaurants()
        {
            var data = SuperAdminService.Instance.getRestaurantList();
            return View(data);
        }
        public ActionResult _RestaurantView(int id)
        {
            using (var context = new ApplicationDbContext())
            {
                var data = context.Restaurants.Where(x => x.Id == id).FirstOrDefault();
                data.Key = HelperService.Instance.Decrypt(data.Key);
                data.CaptainKey = HelperService.Instance.Decrypt(data.CaptainKey);
                return PartialView(data);
            }
        }
        [HttpGet]
        public ActionResult _RestaurantAdd()
        {
            using (var context = new ApplicationDbContext())
            {
                var list = new List<SelectListItem>();
                var pack = context.Packages.ToList();
                for (int i = 0; i < pack.Count; i++)
                {
                    var pdata = new SelectListItem() { Text = pack[i].PName, Value = pack[i].Id.ToString() };
                    list.Add(pdata);
                }
                ViewBag.Menus = list;
                return PartialView();
            }

        }
        [HttpPost]
        public ActionResult _RestaurantAdd(Restaurant restaurant)
        {
            if (!ModelState.IsValid)
            {
                var list = new List<SelectListItem>();
                using(var context = new ApplicationDbContext())
                {
                    var pack = context.Packages.ToList();
                    for (int i = 0; i < pack.Count; i++)
                    {
                        var pdata = new SelectListItem() { Text = pack[i].PName, Value = pack[i].Id.ToString() };
                        list.Add(pdata);
                    }
                    ViewBag.Menus = list;
                }
                return PartialView(restaurant);
            }
            restaurant.IsActive = false;
            using (var context = new ApplicationDbContext())
            {
                var Key = HelperService.Instance.CreatePassword();
                var CaptainKey = HelperService.Instance.CreatePassword();
                restaurant.Key = HelperService.Instance.Encrypt(Key);
                restaurant.CaptainKey = HelperService.Instance.Encrypt(CaptainKey);
                context.Restaurants.Add(restaurant);
                context.SaveChanges();
            }

            return RedirectToAction("Restaurants");
        }
        public ActionResult RestaurantStatus(int restId, bool status)
        {
            var result = false;
            using (var context = new ApplicationDbContext())
            {
                var data = context.Restaurants.FirstOrDefault(x => x.Id == restId);
                if (status == true)
                {
                    var clientPackList = context.ClientPackages.Where(z => z.RCode == data.Id).ToList();
                    ClientPackage clientpack = new ClientPackage();
                    clientpack.RCode = data.Id;
                    clientpack.PCode = data.PDetails;
                    clientpack.PSD = DateTime.Now;
                    var pack = context.Packages.FirstOrDefault(y => y.Id == data.PDetails);
                    int monthval = pack.MV;
                    clientpack.PED = DateTime.Now.AddMonths(monthval);
                    if (clientPackList.Count() == 0)
                    {
                        context.ClientPackages.Add(clientpack);
                        context.SaveChanges();
                    }
                    else
                    {
                        var clipack = context.ClientPackages.FirstOrDefault(w => w.RCode == data.Id);
                        context.Entry(clientpack).State = EntityState.Modified;
                        context.SaveChanges();
                    }
                }
                else
                {
                    var clipacdel = context.ClientPackages.FirstOrDefault(m => m.RCode == data.Id);
                    if (clipacdel != null)
                    {
                        context.ClientPackages.Remove(clipacdel);
                        context.SaveChanges();
                    }
                }
                data.IsActive = status;
                context.Entry(data).State = EntityState.Modified;
                context.SaveChanges();

                result = true;
            }
            return Json(result);
        }
        public ActionResult _RestaurantEdit(int id)
        {
            using (var context = new ApplicationDbContext())
            {
                var data = context.Restaurants.Where(x => x.Id == id).FirstOrDefault();
                var list = new List<SelectListItem>();
                var pack = context.Packages.ToList();
                for (int i = 0; i < pack.Count; i++)
                {
                    var pdata = new SelectListItem() { Text = pack[i].PName, Value = pack[i].Id.ToString() };
                    list.Add(pdata);
                }
                ViewBag.Menus = list;
                if (data.Img != null && data.Img != string.Empty) { TempData["OldImage"] = data.Img; }
                
                return View(data);
            }
        }
        [HttpPost]
        public ActionResult _RestaurantEdit(Restaurant restaurant)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(restaurant);
                }
                using (var context = new ApplicationDbContext())
                {
                    context.Entry(restaurant).State = EntityState.Modified;
                    context.SaveChanges();
                    if (TempData["OldImage"] != null)
                    {
                        if (TempData["OldImage"].ToString() != restaurant.Img)
                        {
                            string removeimagepath = Request.MapPath(TempData["OldImage"].ToString());
                            if (System.IO.File.Exists(removeimagepath))
                            {
                                System.IO.File.Delete(removeimagepath);
                            }
                        }
                    }
                }
                return RedirectToAction("Restaurants");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public bool RestPrefix(string SearchValue)
        {
            var result = false;
            using(var context= new ApplicationDbContext())
            {
                var data = context.Restaurants.FirstOrDefault(x=>x.RPrefix == SearchValue);
                if (data != null)
                {
                    result = true;
                }
            }
            return result;
        }
        #endregion
        #region MenuCategories
        [HttpGet]
        public ActionResult MenuCategories(int id)
        {
            ViewBag.id = id;
            using (var context = new ApplicationDbContext())
            {
                var rest = context.Restaurants.FirstOrDefault(x => x.Id == id);
                ViewBag.RestName = rest.Name;
            }
            var data = SuperAdminService.Instance.getMCatList(id);
            return View(data);
        }
        public ActionResult _MCatAdd(int id)
        {
            var data = new MenuCategory();
            using (var context = new ApplicationDbContext())
            {
                data.RCode = id;
                return View(data);
            }
        }
        [HttpPost]
        public ActionResult _MCatAdd(MenuCategory menuCategory)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var catlist = context.MenuCategory.Where(x => x.MCDesc == menuCategory.MCDesc && x.RCode == menuCategory.RCode).ToList();
                    if (catlist.Count > 0)
                    {
                        ViewBag.errmsg = "Category already exists";
                        var list = new List<SelectListItem>();
                        var pack = context.Restaurants.ToList();
                        for (int i = 0; i < pack.Count; i++)
                        {
                            var rdata = new SelectListItem() { Text = pack[i].Name, Value = pack[i].Id.ToString() };
                            list.Add(rdata);
                        }
                        ViewBag.Menus = list;
                        return PartialView(menuCategory);
                    }
                    else
                    {
                        if (!ModelState.IsValid)
                        {
                            return View(menuCategory);
                        }
                        menuCategory.CreatedOn = DateTime.Now;
                        menuCategory.IsActive = true;

                        context.MenuCategory.Add(menuCategory);
                        context.SaveChanges();
                        return RedirectToAction("MenuCategories", new { id = menuCategory.RCode });
                    }
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public ActionResult McatStatus(int mcId, bool status)
        {
            var result = false;
            using (var context = new ApplicationDbContext())
            {
                var data = context.MenuCategory.FirstOrDefault(x => x.MCCode == mcId);
                data.IsActive = status;
                context.Entry(data).State = EntityState.Modified;
                context.SaveChanges();
                result = true;
            }
            return Json(result);
        }
        public ActionResult _MCatEdit(int id)
        {
            using (var context = new ApplicationDbContext())
            {
                var data = context.MenuCategory.Where(x => x.MCCode == id).FirstOrDefault();
                var list = new List<SelectListItem>();
                var pack = context.Restaurants.ToList();
                for (int i = 0; i < pack.Count; i++)
                {
                    var rdata = new SelectListItem() { Text = pack[i].Name, Value = pack[i].Id.ToString() };
                    list.Add(rdata);
                }
                ViewBag.Menus = list;
                return View(data);
            }
        }

        [HttpPost]
        public ActionResult _MCatEdit(MenuCategory menuCategory)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(menuCategory);

                }
                using (var context = new ApplicationDbContext())
                {
                    context.Entry(menuCategory).State = EntityState.Modified;
                    context.SaveChanges();
                }
                return RedirectToAction("MenuCategories", new { id = menuCategory.RCode });
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion
        #region Menu
        public ActionResult Menu(int id)
        {
            var result = new List<AdminMenuViewModel>();
            using (var context = new ApplicationDbContext())
            {
                var rest = context.Restaurants.FirstOrDefault(x => x.Id == id);
                ViewBag.RestName = rest.Name;
                ViewBag.rid = id;
                result = MenuDetailsService.Instance.GetRestMenusByRCode(id).ToList();
            }
            return View(result);
        }
        public ActionResult AddMenu(int rid)
        {
            using (var context = new ApplicationDbContext())
            {
                var tlist = new List<SelectListItem>();
                var type = context.MenuTypes.Where(x=>x.IsActive == true).OrderByDescending(x => x.MTCode).ToList();
                for (int i = 0; i < type.Count; i++)
                {
                    var tdata = new SelectListItem() { Text = type[i].MTDesc, Value = type[i].MTCode.ToString() };
                    tlist.Add(tdata);
                }
                var catList = new List<SelectListItem>();
                var category = context.MenuCategory.Where(x=>x.RCode == rid && x.IsActive == true).OrderByDescending(x => x.MCCode).ToList();
                for (int i = 0; i < category.Count; i++)
                {
                    var catData = new SelectListItem() { Text = category[i].MCDesc, Value = category[i].MCCode.ToString() };
                    catList.Add(catData);
                }
                ViewBag.menuCatItem = catList;
                ViewBag.tp = tlist;
                ViewBag.rid = rid;
                return View();
            }
        }
        [HttpPost]
        public ActionResult AddMenu(Menu menu)
        {
            using (var context = new ApplicationDbContext())
            {
                var repmenu = context.Menu.Where(x => x.MDesc == menu.MDesc && x.RCode == menu.RCode);
                var tlist = new List<SelectListItem>();
                var type = context.MenuTypes.Where(x => x.IsActive == true).OrderByDescending(x => x.MTCode).ToList();
                for (int i = 0; i < type.Count; i++)
                {
                    var tdata = new SelectListItem() { Text = type[i].MTDesc, Value = type[i].MTCode.ToString() };
                    tlist.Add(tdata);
                }
                var catList = new List<SelectListItem>();
                var category = context.MenuCategory.Where(x => x.RCode == menu.RCode && x.IsActive == true).OrderByDescending(x => x.MCCode).ToList();
                for (int i = 0; i < category.Count; i++)
                {
                    var catData = new SelectListItem() { Text = category[i].MCDesc, Value = category[i].MCCode.ToString() };
                    catList.Add(catData);
                }
                ViewBag.menuCatItem = catList;
                ViewBag.tp = tlist;
                ViewBag.rid = menu.RCode;
                if (repmenu.Count() > 0)
                {
                    ViewBag.ErrMsg = "Menu Item already exists";
                    return View(menu);
                }
                else
                {
                    if (!ModelState.IsValid)
                    {
                        return View(menu);
                    }
                    menu.CreatedOn = DateTime.Now;
                    menu.IsActive = true;
                    menu.Rating = menu.Rating != null && menu.Rating != string.Empty ? menu.Rating : "3";
                    context.Menu.Add(menu);
                    context.SaveChanges();
                    return RedirectToAction("Menu", new { id = menu.RCode });

                }
            }
        }
        public ActionResult MenuStatus(int menuId, bool status)
        {
            var result = false;
            using (var context = new ApplicationDbContext())
            {
                var data = context.Menu.FirstOrDefault(x => x.MCode == menuId);
                data.IsActive = status;
                context.Entry(data).State = EntityState.Modified;
                context.SaveChanges();
                result = true;
            }
            return Json(result);
        }
        public ActionResult _MenuEdit(int id)
        {
            using (var context = new ApplicationDbContext())
            {
                var data = context.Menu.FirstOrDefault(x => x.MCode == id);
                var list = new List<SelectListItem>();
                var menu = context.MenuCategory.Where(x=>x.RCode == data.RCode && x.IsActive == true).ToList();
                for (int i = 0; i < menu.Count; i++)
                {
                    var rdata = new SelectListItem() { Text = menu[i].MCDesc, Value = menu[i].MCCode.ToString() };
                    list.Add(rdata);
                }
                var tlist = new List<SelectListItem>();
                var type = context.MenuTypes.OrderByDescending(x => x.MTCode).ToList();
                for (int i = 0; i < type.Count; i++)
                {
                    var tdata = new SelectListItem() { Text = type[i].MTDesc, Value = type[i].MTCode.ToString() };
                    tlist.Add(tdata);
                }
                if (data.img != null && data.img != string.Empty) { TempData["OldImage"] = data.img; }
                ViewBag.tp = tlist;
                ViewBag.cat = list;
                return View(data);
            }
        }
        [HttpPost]
        public ActionResult _MenuEdit(Menu menu)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(menu);
                }
                using (var context = new ApplicationDbContext())
                {
                    context.Entry(menu).State = EntityState.Modified;
                    context.SaveChanges();
                    if (TempData["OldImage"] != null)
                    {
                        if (TempData["OldImage"].ToString() != menu.img)
                        {
                            string removeimagepath = Request.MapPath(TempData["OldImage"].ToString());
                            if (System.IO.File.Exists(removeimagepath))
                            {
                                System.IO.File.Delete(removeimagepath);
                            }
                        }
                    }
                }
                return RedirectToAction("Menu", new { id = menu.RCode });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        #region User
        public ActionResult RestaurantUsers()
        {
            try
            {
                var data = UserService.Instance.GetUsers();
                return View(data);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
        #region CleanData
        public void ClearSuperAdminCartData()
        {
            using(var context = new ApplicationDbContext())
            {
                var Cartdata = context.Cart.Include(x=>x.CartItems).ToList();
                var cartItems = new List<CartItems>();
                var CartItemdata = Cartdata.Select(x => {
                    cartItems = x.CartItems;
                    return x;
                });
                context.CartItems.RemoveRange(cartItems);
                context.Cart.RemoveRange(Cartdata);
                context.SaveChanges();
            }
        }
        #endregion
    }
}