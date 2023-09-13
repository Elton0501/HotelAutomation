using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class CheckOutViewModel
    {
        public string TableNo { get; set; }
        public User UserDetail { get; set; }
        public Restaurant RestaurantDetail { get; set; }
        public string Comment { get; set; }
    }
}
