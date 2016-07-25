using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DMS_WebAPI.DBModel
{
    public class AdminLanguageValues
    {
        public int Id { get; set; }
        public int LanguageId { get; set; }
        [MaxLength(2000)]
        public string Label { get; set; }
        [MaxLength(2000)]
        public string Value { get; set; }
        [ForeignKey("LanguageId")]
        public virtual AdminLanguages Language { get; set; }
    }
}