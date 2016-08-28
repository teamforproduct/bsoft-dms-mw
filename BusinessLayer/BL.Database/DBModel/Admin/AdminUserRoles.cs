using System;

namespace BL.Database.DBModel.Admin
{
    using Dictionary;
    using global::System.ComponentModel.DataAnnotations.Schema;

    public class AdminUserRoles
    {
        public int Id { get; set; }
        [Index("IX_UserRoleStartDate", 1, IsUnique = true)]
        public int UserId { get; set; }
        [Index("IX_UserRoleStartDate", 2, IsUnique = true)]
        [Index("IX_RoleId", 1)]
        public int RoleId { get; set; }
        [Index("IX_UserRoleStartDate", 3, IsUnique = true)]
        public DateTime StartDate { get; set; }
        [Index("IX_UserRoleStartDate", 4, IsUnique = true)]
        public int? PositionId { get; set; }
        public DateTime EndDate { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }

        //public virtual AdminUsers User { get; set; }
        [ForeignKey("RoleId")]
        public virtual AdminRoles Role { get; set; }
        [ForeignKey("UserId")]
        public virtual DictionaryAgents Agent { get; set; }
        [ForeignKey("PositionId")]
        public virtual DictionaryPositions Position { get; set; }
    }
}
