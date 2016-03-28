using System;

namespace BL.Database.DBModel.Admin
{
    using Dictionary;
    using global::System.ComponentModel.DataAnnotations.Schema;

    public class AdminUserRoles
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }

        //public virtual AdminUsers User { get; set; }
        [ForeignKey("RoleId")]
        public virtual AdminRoles Role { get; set; }
        [ForeignKey("UserId")]
        public virtual DictionaryAgents Agent { get; set; }
    }
}
