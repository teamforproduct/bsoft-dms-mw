using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Database.DBModel.Dictionary
{
    public class DictionaryLinkTypes
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        [MaxLength(400)]
        public string Code { get; set; }   
        public bool IsImportant { get; set; }
    }
}
