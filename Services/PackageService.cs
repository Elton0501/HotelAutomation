using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class PackageService
    {
        #region singleton
        public static PackageService Instance
        {
            get
            {
                if (instance == null) instance = new PackageService();
                return instance;
            }
        }
        private static PackageService instance { get; set; }

        public PackageService()
        {
        }
        #endregion
    }
}
