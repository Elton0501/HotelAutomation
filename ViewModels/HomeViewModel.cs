using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class HomeViewModel
    {
        public IEnumerable<MenuDetailsViewModel> MenuModel { get; set; }
        public string SearchValue { get; set; }
        public string MenuHeading { get; set; }
        public int? CategoryType { get; set; }
        public string CategoryTypeName { get; set; }
        public IEnumerable<MenuCategory> menuCatagories { get; set; }
        public IEnumerable<CartItems> CartModel { get; set; }
        public bool Veg { get; set; }
        public bool NonVeg { get; set; }
        public bool Bar { get; set; }
        public bool Beverages { get; set; }
        public bool IsUserAdmin { get; set; }
        public string UserAdmin { get; set; }
    }
}
