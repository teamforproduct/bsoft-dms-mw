using System;
using System.ComponentModel.DataAnnotations.Schema;
using BL.Database.DBModel.Dictionary;
using System.ComponentModel.DataAnnotations;

namespace BL.Database.DBModel.Admin
{
    public class AdminLanguageValues
    {
        public int Id { get; set; }
        [Index("IX_Label", 2, IsUnique = true)]
        [Index("IX_LanguageId", 1)]
        public int LanguageId { get; set; }
        [MaxLength(2000)]
        [Index("IX_Label", 1, IsUnique = true)]
        public string Label { get; set; }
        [MaxLength(2000)]
        public string Value { get; set; }
        [ForeignKey("LanguageId")]
        public virtual AdminLanguages Language { get; set; }
    }
}