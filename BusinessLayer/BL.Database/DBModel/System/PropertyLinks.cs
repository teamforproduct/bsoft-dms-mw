using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Database.DBModel.System
{
    public partial class PropertyLinks
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public int ObjectId { get; set; }
        public string Filers { get; set; }
        public bool IsMandatory { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        [ForeignKey("PropertyId")]
        public virtual Properties Property { get; set; }
        [ForeignKey("ObjectId")]
        public virtual SystemObjects Object { get; set; }
    }
}
