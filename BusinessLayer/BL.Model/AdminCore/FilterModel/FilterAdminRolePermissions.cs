using BL.Model.Common;
using System.Collections.Generic;

namespace BL.Model.AdminCore.FilterModel
{
    /// <summary>
    /// Фильтры FilterAdminUserPermissions
    /// </summary>
    public class FilterAdminRolePermissions : BaseFilter
    {

        /// <summary>
        /// Permissions
        /// </summary>
        public List<int> PermissionIDs { get; set; }

        /// <summary>
        /// Role
        /// </summary>
        public List<int> RoleIDs { get; set; }

    }
}