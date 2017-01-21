using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Database.DBModel.System
{
    public partial class SystemFeatures
    {
        public int Id { get; set; }

        public int ModuleId { get; set; }

        [MaxLength(400)]
        public string Code { get; set; }

        [MaxLength(2000)]
        public string Name { get; set; }

        public int Order { get; set; }

        [ForeignKey("ModuleId")]
        public virtual SystemModules Module { get; set; }
    }
}
