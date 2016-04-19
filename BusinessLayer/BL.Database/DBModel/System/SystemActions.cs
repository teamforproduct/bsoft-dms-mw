using System;
using System.Collections.Generic;

namespace BL.Database.DBModel.System
{
    using Admin;
    using global::System.ComponentModel.DataAnnotations;
    using global::System.ComponentModel.DataAnnotations.Schema;


    public partial class SystemActions
    {
        public SystemActions()
        {
            this.RoleActions = new HashSet<AdminRoleActions>();
            this.GarantableActions = new HashSet<SystemActions>();
        }

        public int Id { get; set; }
        [Index("IX_ObjectCode", 1, IsUnique = true)]
        public int ObjectId { get; set; }
        [MaxLength(2000)]
        [Index("IX_ObjectCode", 2, IsUnique = true)]
        public string Code { get; set; }
        [MaxLength(2000)]
        public string API { get; set; }
        [MaxLength(2000)]
        public string Description { get; set; }
        [MaxLength(2000)]
        public string Category { get; set; }
        public bool IsGrantable { get; set; }
        public bool IsGrantableByRecordId { get; set; }
        public bool IsVisible { get; set; }
        public Nullable<int> GrantId { get; set; }
        [ForeignKey("ObjectId")]
        public virtual SystemObjects Object { get; set; }

        public virtual ICollection<AdminRoleActions> RoleActions { get; set; }
        [ForeignKey("GrantId")]
        public virtual SystemActions GrantAction { get; set; }

        public virtual ICollection<SystemActions> GarantableActions { get; set; }
    }
}
