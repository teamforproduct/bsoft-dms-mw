using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMS_WebAPI.DBModel
{
    public class SystemControlQuestions
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        
        [MaxLength(200)]
        [Index("IX_FileName", 1, IsUnique = true)]
        public string Name { get; set; }

    }
}