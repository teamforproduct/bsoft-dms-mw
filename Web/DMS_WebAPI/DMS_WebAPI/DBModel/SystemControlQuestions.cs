using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMS_WebAPI.DBModel
{
    public class SystemControlQuestions
    {
        /// <summary>
        /// Цифровой код языка
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        
        /// <summary>
        /// Язык
        /// </summary>
        [MaxLength(200)]
        [Index("IX_FileName", 1, IsUnique = true)]
        public string Name { get; set; }

    }
}