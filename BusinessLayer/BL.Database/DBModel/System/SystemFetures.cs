using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Database.DBModel.System
{
    public partial class SystemFeatures2
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Index("IX_ModuleCode", 1, IsUnique = true)]
        public int ModuleId { get; set; }

        [MaxLength(400)]
        [Index("IX_ModuleCode", 2, IsUnique = true)]
        public string Code { get; set; }

        [MaxLength(2000)]
        public string Name { get; set; }

        public int Order { get; set; }

        [ForeignKey("ModuleId")]
        public virtual SystemModules2 Module { get; set; }
    }
}
