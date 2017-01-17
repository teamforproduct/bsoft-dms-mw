using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Database.DBModel.System
{
    public partial class SystemModuleFetures
    {
        public int Id { get; set; }

        [MaxLength(400)]
        public string Module { get; set; }

        [MaxLength(400)]
        public string Feature { get; set; }

        [MaxLength(2000)]
        public string Name { get; set; }

        public int Order { get; set; }
    }
}
