using BL.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.AdminCore.FilterModel
{
    /// <summary>
    /// Фильтры FilterAdminUserPermissions
    /// </summary>
    public class FilterAdminRolePermissions : AdminBaseFilterParameters
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