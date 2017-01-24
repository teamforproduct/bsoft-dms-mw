namespace BL.Model.AdminCore.FilterModel
{
    /// <summary>
    /// Фильтры FilterAdminUserPermissions
    /// </summary>
    public class FilterAdminRolePermissionsDIP
    {

        /// <summary>
        /// Role
        /// </summary>
        public int RoleId { get; set; }

        public bool IsChecked { get; set; }

        public string Module { get; set; }

        public string Feature { get; set; }

    }
}