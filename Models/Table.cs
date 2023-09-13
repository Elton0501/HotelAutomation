using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Table : Base
    {
        public int Id { get; set; }
        public string TCode { get; set; }
        public string RCode { get; set; }
    }
}
