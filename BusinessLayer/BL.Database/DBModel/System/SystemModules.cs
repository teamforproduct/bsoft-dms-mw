using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Database.DBModel.System
{
    public partial class SystemModules
    {
        public int Id { get; set; }

        [MaxLength(400)]
        [Index("IX_Code", 1, IsUnique = true)]
        public string Code { get; set; }

        [MaxLength(2000)]
        public string Name { get; set; }

        public int Order { get; set; }
    }
}
