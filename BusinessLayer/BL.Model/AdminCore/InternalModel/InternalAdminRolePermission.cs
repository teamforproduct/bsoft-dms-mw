using BL.Model.Common;

namespace BL.Model.AdminCore.InternalModel
{
    public class InternalAdminRolePermission : LastChangeInfo
    {

        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        public int RoleId { get; set; }

        public int PermissionId { get; set; }

        /// <summary>
        /// ИД клиента
        /// </summary>
        public int ClientId { get; set; }

    }
}