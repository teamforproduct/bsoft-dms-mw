using System;
using System.Collections.Generic;

namespace BL.Database.DBModel.Admin
{
    using Dictionary;
    using global::System.ComponentModel.DataAnnotations.Schema;

    public partial class AdminRoles
    {
        public AdminRoles()
        {
            this.UserRoles = new HashSet<AdminUserRoles>();
            this.RoleActions = new HashSet<AdminRoleActions>();
            this.PositionRoles = new HashSet<AdminPositionRoles>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int PositionId { get; set; }
        public int AccessLevelId { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }

        public virtual ICollection<AdminUserRoles> UserRoles { get; set; }
        public virtual ICollection<AdminRoleActions> RoleActions { get; set; }

        public virtual ICollection<AdminPositionRoles> PositionRoles { get; set; }
    }
}
