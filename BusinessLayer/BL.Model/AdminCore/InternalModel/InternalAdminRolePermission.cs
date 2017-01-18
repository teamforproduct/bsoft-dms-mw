using BL.Model.Common;
using BL.Model.AdminCore.IncomingModel;
using BL.Model.Enums;

namespace BL.Model.AdminCore.InternalModel
{
    public class InternalAdminRolePermission : LastChangeInfo
    {
        public InternalAdminRolePermission()
        { }

        public InternalAdminRolePermission(SetRolePermission model)
        {
            RoleId = model.RoleId;
            PermissionId = model.PermissionId;
        }

        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        public int RoleId { get; set; }

        public int PermissionId { get; set; }

    }
}