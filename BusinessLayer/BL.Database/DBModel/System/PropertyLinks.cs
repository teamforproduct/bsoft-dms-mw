using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Database.DBModel.System
{
    public partial class PropertyLinks
    {
        public int Id { get; set; }
        [Index("IX_ObjectPropertyFilers", 2, IsUnique = true)]
        [Index("IX_PropertyId", 1)]
        public int PropertyId { get; set; }
        [Index("IX_ObjectPropertyFilers", 1, IsUnique = true)]
        public int ObjectId { get; set; }
        [MaxLength(2000)]
        [Index("IX_ObjectPropertyFilers", 3, IsUnique = true)]
        public string Filers { get; set; }
        public bool IsMandatory { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        [ForeignKey("PropertyId")]
        public virtual Properties Property { get; set; }
        [ForeignKey("ObjectId")]
        public virtual SystemObjects Object { get; set; }
        public virtual ICollection<PropertyValues> PropertyValues { get; set; }
    }
}
