﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Database.DBModel.System
{


    public class SystemFields
    {
        public int Id { get; set; }
        [Index("IX_ObjectCode", 1, IsUnique = true)]
        public int ObjectId { get; set; }
        [MaxLength(2000)]
        [Index("IX_ObjectCode", 2, IsUnique = true)]
        public string Code { get; set; }
        [MaxLength(2000)]
        public string Description { get; set; }
        public int ValueTypeId { get; set; }
        [ForeignKey("ObjectId")]
        public virtual SystemObjects Object { get; set; }
        [ForeignKey("ValueTypeId")]
        public virtual SystemValueTypes ValueTypes { get; set; }
    }
}
