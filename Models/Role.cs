using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Role : Base
    {
        public int Id { get; set; }
        public Guid RoleId { get; set; }
        public string RoleName { get; set; }
    }
}
