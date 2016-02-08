using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Database.DBModel.System
{


    public class SystemFields
    {
        public int Id { get; set; }
        public int ObjectId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int ValueTypeId { get; set; }
        [ForeignKey("ObjectId")]
        public virtual SystemObjects Object { get; set; }
        [ForeignKey("ValueTypeId")]
        public virtual SystemValueTypes ValueTypes { get; set; }
    }
}
