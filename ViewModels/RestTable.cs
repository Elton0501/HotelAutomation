using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class RestTable
    {
        public IEnumerable<Table> Tables { get; set; }
        public IEnumerable<Table> UserTables { get; set; }
        public string TableAvail { get; set; }
        public string TableRese { get; set; }
    }
}
