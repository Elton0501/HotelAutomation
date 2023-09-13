using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class KOTViewModel
    {
        public string Complimentary { get; set; }
        public bool IsComplimentary { get; set; }
        public bool IsOld { get; set; }
        public int RCode { get; set; }
        public string PrinterCode { get; set; }
        public int tempToPrint { get; set; }
        public List<BillItem> BillItems { get; set; }
        public string RName { get; set; }
        public string TNumber { get; set; }
        public string Comment { get; set; }
    }
}
