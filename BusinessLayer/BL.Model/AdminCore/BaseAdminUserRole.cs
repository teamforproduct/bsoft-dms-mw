using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.AdminCore
{
    public class BaseAdminUserRole
    {
        public int? Id { get; set; }
        public int? UserId { get; set; }
        public int? RoleId { get; set; }
        public int? LastChangeUserId { get; set; }
        public DateTime? LastChangeDate { get; set; }
        public string RoleName { get; set; }
        public string UserName { get; set; }
        public int? RolePositionId { get; set; }
        public string RolePositionName { get; set; }
        public string RolePositionExecutorAgentName { get; set; }
    }
}
