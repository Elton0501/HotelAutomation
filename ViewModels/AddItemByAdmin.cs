using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class AddItemByAdmin
    {
        public IEnumerable<Menu> Menus { get; set; }
        public IEnumerable<MenuCategory> MenuCategories { get; set; }
        public string TCode{ get; set; }
        public string RCode{ get; set; }
        public string UCode{ get; set; }
        public bool isNew { get; set; }
        public bool isPlacedOrder { get; set; }
    }
}
