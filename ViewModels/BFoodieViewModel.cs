using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class BFoodieViewModel
    {
        public IEnumerable<Restaurant> Restaurants { get; set; }
        public IEnumerable<Restaurant> RestaurantsRecommended { get; set; }
        public IEnumerable<Restaurant> RestaurantsTrending { get; set; }
    }
}
