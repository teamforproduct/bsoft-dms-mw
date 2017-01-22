using System.ComponentModel.DataAnnotations;

namespace BL.Database.DBModel.System
{
    public partial class SystemModules
    {
        public int Id { get; set; }

        [MaxLength(400)]
        public string Code { get; set; }

        [MaxLength(2000)]
        public string Name { get; set; }

        public int Order { get; set; }
    }
}
