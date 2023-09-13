using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class UserOrderService
    {
        #region singleton
        public static UserOrderService Instance
        {
            get
            {
                if (instance == null) instance = new UserOrderService();
                return instance;
            }
        }
        private static UserOrderService instance { get; set; }

        public UserOrderService()
        {
        }
        #endregion
    }
}
