using System;
using System.Collections.Generic;

namespace BL.Database.DBModel.System
{
    using global::System.ComponentModel.DataAnnotations;
    using global::System.ComponentModel.DataAnnotations.Schema;


    public partial class SystemActions
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public Nullable<int> PermissionId { get; set; }

        public int ObjectId { get; set; }

        [MaxLength(400)]
        public string Code { get; set; }

        public int? CategoryId { get; set; }

        [ForeignKey("PermissionId")]
        public virtual SystemPermissions Permission { get; set; }

        [ForeignKey("ObjectId")]
        public virtual SystemObjects Object { get; set; }
    }
}
