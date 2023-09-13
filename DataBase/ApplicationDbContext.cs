using Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() : base("HotelAutomationDb")
        {

        }
        public virtual DbSet<Package> Packages { get; set; }
        public virtual DbSet<ClientPackage> ClientPackages { get; set; }
        public virtual DbSet<PlaceOrder> PlaceOrders { get; set; }
        public virtual DbSet<PlaceOrderItems> PlaceOrderItems { get; set; }
        public virtual DbSet<Menu> Menu { get; set; }
        public virtual DbSet<MenuCategory> MenuCategory { get; set; }
        public virtual DbSet<Table> Table { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<Cart> Cart { get; set; }
        public virtual DbSet<CartItems> CartItems { get; set; }
        public virtual DbSet<Restaurant> Restaurants { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<MenuType> MenuTypes { get; set; }
        public virtual DbSet<EmailOTP> EmailOTP { get; set; }
        public virtual DbSet<Orders> Orders { get; set; }
        public virtual DbSet<OrderItem> OrderItem { get; set; }
        public virtual DbSet<BillPrintOrder> BillPrintOrders { get; set; }
        public virtual DbSet<Inventory> Inventories { get; set; }
        public virtual DbSet<CaptainDetails> CaptainDetails { get; set; }
        public virtual DbSet<BillDiscount> BillDiscount { get; set; }
        public virtual DbSet<CouponApplied> CouponApplied { get; set; }
        public virtual DbSet<Notification> Notification { get; set; }

    }
}
