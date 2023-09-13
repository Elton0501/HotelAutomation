using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class MenuDetailsViewModel
    {
        public int MCode { get; set; }
        public int MCCode { get; set; }
        public string MDesc { get; set; }
        public string MComment { get; set; }
        public string MCDesc { get; set; }
        public decimal Amount { get; set; }
        public decimal DisAmount { get; set; }
        public int Discount { get; set; }
        public string MenuType { get; set; }
        public string Image { get; set; }
        public bool isAvailCart { get; set; }

    }
    public class AdminMenuViewModel
    {
        public int MCode { get; set; }
        public int MCCode { get; set; }
        public string MDesc { get; set; }
        public string MCDesc { get; set; }
        public decimal Amount { get; set; }
        public string MenuType { get; set; }
        public string Image { get; set; }
        public string CreatedOn { get; set; }
        public string Discount { get; set; }
        public string HDDiscount { get; set; }
        public string TADiscount { get; set; }
        public string MComments { get; set; }
        public bool IsActive { get; set; }

    }
    public class DropdownMenuViewModel
    {
        public int MCode { get; set; }
        public string MDesc { get; set; }
        public string Discount { get; set; }
    }
    public class AdminPlaceOrderModel
    {
        [Required]
        public string MCode { get; set; }
        [Required]
        public string CreatedBy { get; set; }
        public string Comments { get; set; }
        public string PunchedBy { get; set; }
        [Required]
        public string TableCode { get; set; }
        [Required]
        public string RestCode { get; set; }
        public string Discount { get; set; }
        public bool isNew { get; set; }
    }
}
