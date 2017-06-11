using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Database.DBModel.System
{


    public class SystemFields
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public int ObjectId { get; set; }

        [MaxLength(400)]
        public string Code { get; set; }

        public int ValueTypeId { get; set; }

        [ForeignKey("ObjectId")]
        public virtual SystemObjects Object { get; set; }

        [ForeignKey("ValueTypeId")]
        public virtual SystemValueTypes ValueTypes { get; set; }
    }
}
