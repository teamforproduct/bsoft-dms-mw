using System;
using System.ComponentModel.DataAnnotations.Schema;
using BL.Database.DBModel.Dictionary;

namespace BL.Database.DBModel.Admin
{
    public class AdminLanguageValues
    {
        public int Id { get; set; }
        public int LanguageId { get; set; }
        public string Label { get; set; }
        public string Value { get; set; }
        [ForeignKey("LanguageId")]
        public virtual AdminLanguages Language { get; set; }
    }
}