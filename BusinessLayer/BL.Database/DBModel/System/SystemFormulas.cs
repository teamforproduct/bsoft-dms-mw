using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Database.DBModel.System
{

    public class SystemFormulas
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [MaxLength(400)]
        [Index("IX_Code", 1, IsUnique = true)]
        public string Code { get; set; }

        [MaxLength(2000)]
        public string Name { get; set; }    //TODO DEL!!!

        [MaxLength(2000)]
        public string Description { get; set; } //TODO DEL!!!

        [MaxLength(2000)]
        public string Example { get; set; }
    }
}
