using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class ClientDetailsService
    {
        #region singleton
        public static ClientDetailsService Instance
        {
            get
            {
                if (instance == null) instance = new ClientDetailsService();
                return instance;
            }
        }
        private static ClientDetailsService instance { get; set; }

        public ClientDetailsService()
        {
        }
        #endregion
    }
}
