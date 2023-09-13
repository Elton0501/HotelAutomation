using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class BillPrintOrder
    {
        [Key]
        public int Id { get; set; }
        public string OrderId { get; set; }
        public int RCode { get; set; }
        public bool isBill { get; set; }
        //only use for demo bills during home delivery
        public bool isPlaceOrder { get; set; }
        public DateTime CreatedOn { get; set; }
        [NotMapped]
        public IEnumerable<PlaceOrder> PlaceOrders { get; set; }
    }
}
