using System;

namespace BL.Database.DBModel.Admin
{
    using Dictionary;
    using global::System.ComponentModel.DataAnnotations.Schema;

    public class AdminUserRoles
    {
        public int Id { get; set; }
        [Index("IX_UserRoleExecutor", 1, IsUnique = true)]
        public int? UserId { get; set; }
        [Index("IX_UserRoleExecutor", 2, IsUnique = true)]
        [Index("IX_RoleId", 1)]
        public int RoleId { get; set; }
        //public DateTime StartDate { get; set; }
        //[Index("IX_UserRoleStartDate", 4, IsUnique = true)]
        [Index("IX_UserRoleExecutor", 3, IsUnique = true)]
        [Index("IX_PositionExecutorId", 1)]
        public int? PositionExecutorId { get; set; }
        //public int? PositionId { get; set; }
        //public DateTime EndDate { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }

        //public virtual AdminUsers User { get; set; }
        [ForeignKey("RoleId")]
        public virtual AdminRoles Role { get; set; }
        [ForeignKey("UserId")]
        public virtual DictionaryAgents Agent { get; set; }
        //[ForeignKey("PositionId")]
        //public virtual DictionaryPositions Position { get; set; }
        [ForeignKey("PositionExecutorId")]
        public virtual DictionaryPositionExecutors PositionExecutor { get; set; }
    }
}
