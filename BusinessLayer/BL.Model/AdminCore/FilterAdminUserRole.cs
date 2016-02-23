using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.AdminCore
{
    public class FilterAdminUserRole
    {
        public List<int> UserRoleId { get; set; }
        public List<int> UserId { get; set; }
        public List<int> RoleId { get; set; }

    }
}
