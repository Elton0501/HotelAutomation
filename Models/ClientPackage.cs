using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class ClientPackage
    {
        public int Id { get; set; }
        public int RCode { get; set; }
        public int? PCode { get; set; }//Package Code
        public DateTime PSD { get; set; }//Package start date
        public DateTime PED { get; set; }//Package end date
        //Advanve Package
        public bool IsAPackage { get; set; }
    }
}
