using System.Collections.Generic;
using BL.Model.AdminCore.InternalModel;
using BL.Model.SystemCore.InternalModel;

namespace BL.Model.AdminCore
{
    public class AdminAccessInfo
    {
        public List<InternalAdminRoleAction> ActionAccess { get; set; }
        public List<InternalAdminRolePermission> RolePermissions { get; set; }
        public List<InternalAdminUserRole> UserRoles { get; set; }
        public List<InternalAdminRole> Roles { get; set; }
        public List<InternalAdminPositionRole> PositionRoles { get; set; }
        public List<InternalSystemAction> Actions { get; set; }
    }
}