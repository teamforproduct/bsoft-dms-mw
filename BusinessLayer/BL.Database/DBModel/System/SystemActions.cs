﻿using System;
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

        [Index("IX_ObjectCode", 1, IsUnique = true)]
        public int ObjectId { get; set; }

        [MaxLength(400)]
        [Index("IX_ObjectCode", 2, IsUnique = true)]
        public string Code { get; set; }

        [MaxLength(2000)]
        public string Description { get; set; } //TODO DEL!!!

        [MaxLength(2000)]
        public string Category { get; set; }    //TODO DEL!!!
        public int? CategoryId { get; set; }

        [ForeignKey("PermissionId")]
        public virtual SystemPermissions Permission { get; set; }

        [ForeignKey("ObjectId")]
        public virtual SystemObjects Object { get; set; }
    }
}
