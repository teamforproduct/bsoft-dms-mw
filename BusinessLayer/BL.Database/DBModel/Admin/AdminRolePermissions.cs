using System;

namespace BL.Database.DBModel.Admin
{
    using global::System.ComponentModel.DataAnnotations.Schema;
    using System;

    public class AdminRolePermissions
    {
        public int Id { get; set; }

        [Index("IX_PermissionRole", 2, IsUnique = true)]
        public int RoleId { get; set; }

        [Index("IX_PermissionRole", 1, IsUnique = true)]
        public int PermissionId { get; set; }

        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }



        [ForeignKey("RoleId")]
        public virtual AdminRoles Role { get; set; }

        [ForeignKey("PermissionId")]
        public virtual SystemPermissions Permission { get; set; }
    }
}
