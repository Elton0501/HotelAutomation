using DataBase;
using Models;
using Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace HotelAutomation.Web.App_Start
{
    public class AutomateRole
    {
        public static void createRolesandUsers()
        {
            var context = new ApplicationDbContext();
            var dataRole = context.Roles.ToList();
            if (dataRole.Count == 0)
            {
                string role = ConfigurationManager.AppSettings["Role"];
                string[] roles = role.Split(',');
                foreach (var item in roles)
                {
                    Role model = new Role();
                    model.RoleId = Guid.NewGuid();
                    model.RoleName = item;
                    model.CreatedOn = DateTime.Now;
                    model.CreatedBy = "Super Admin";
                    model.IsActive = true;
                    context.Roles.Add(model);
                    context.SaveChanges();
                }
                //Save SuperAdmin
                var user = new User();
                user.CreatedBy = "Super Admin";
                user.CreatedOn = DateTime.Now;
                user.Name = "Super Admin";
                user.Varified = true;
                user.RoleId = context.Roles.FirstOrDefault(x => x.RoleName == "Super Admin").RoleId;
                user.Email = ConfigurationManager.AppSettings["SuperAdminEmail"];
                user.Password = HelperService.Instance.Encrypt(ConfigurationManager.AppSettings["SuperAdminPassword"].ToString());
                user.IsActive = true;
                context.User.Add(user);
                context.SaveChanges();
            }
        }
    }
}