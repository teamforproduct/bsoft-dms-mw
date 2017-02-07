using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Database.DBModel.Admin
{
    public partial class AdminRoles
    {
        public AdminRoles()
        {
            this.UserRoles = new HashSet<AdminUserRoles>();
            this.PositionRoles = new HashSet<AdminPositionRoles>();
            this.RolePermissions = new HashSet<AdminRolePermissions>();
        }

        public int Id { get; set; }
        [Index("IX_Name", 2, IsUnique = true)]
        [Index("IX_ClientId", 1)]
        public int ClientId { get; set; }
        [MaxLength(400)]
        [Index("IX_Name", 1, IsUnique = true)]
        public string Name { get; set; }
        [MaxLength(2000)]
        public string Description { get; set; }
        public int? RoleTypeId { get; set; }
        //        public int PositionId { get; set; }
        //        public int AccessLevelId { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }

        [ForeignKey("RoleTypeId")]
        public virtual AdminRoleTypes RoleType { get; set; }

        public virtual ICollection<AdminUserRoles> UserRoles { get; set; }
        public virtual ICollection<AdminRolePermissions> RolePermissions { get; set; }

        public virtual ICollection<AdminPositionRoles> PositionRoles { get; set; }
    }
}
