using System;
using System.ComponentModel.DataAnnotations.Schema;
using BL.Database.DBModel.Dictionary;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BL.Database.DBModel.Admin
{
    public class AdminLanguages
    {
        public AdminLanguages()
        {
            this.LanguageValues = new HashSet<AdminLanguageValues>();
        }
        public int Id { get; set; }
        [MaxLength(2000)]
        [Index("IX_Code", 1, IsUnique = true)]
        public string Code { get; set; }
        [MaxLength(2000)]
        [Index("IX_Name", 1, IsUnique = true)]
        public string Name { get; set; }
        public bool IsDefault { get; set; }
        public virtual ICollection<AdminLanguageValues> LanguageValues { get; set; }
    }
}