using System;

namespace BL.Database.DBModel.Admin
{
    using global::System.ComponentModel.DataAnnotations.Schema;
    using System;

    public class AdminRoleActions
    {
        public int Id { get; set; }
        [Index("IX_ActionRoleRecord", 2, IsUnique = true)]
        [Index("IX_RoleId", 1)]
        public int RoleId { get; set; }
        [Index("IX_ActionRoleRecord", 1, IsUnique = true)]
        public int ActionId { get; set; }
        [Index("IX_ActionRoleRecord", 3, IsUnique = true)]
        public Nullable<int> RecordId { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }



        [ForeignKey("RoleId")]
        public virtual AdminRoles Role { get; set; }
        [ForeignKey("ActionId")]
        public virtual SystemActions Action { get; set; }
    }
}
