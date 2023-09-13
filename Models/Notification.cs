using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool isMultipleUser { get; set; }
        public string RCode { get; set; }
        public string TypeId { get; set; }
        public string AnchorLink { get; set; }
        [NotMapped]
        public string TimeTrek { get; set; }
    }
}
