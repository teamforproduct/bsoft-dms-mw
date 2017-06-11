using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Database.DBModel.System
{

    public class SystemFormulas
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [MaxLength(400)]
        public string Code { get; set; }
        [MaxLength(2000)]
        public string Example { get; set; }
    }
}
