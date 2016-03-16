using System;
using System.ComponentModel.DataAnnotations.Schema;
using BL.Database.DBModel.Dictionary;
using System.Collections.Generic;

namespace BL.Database.DBModel.Admin
{
    public class AdminLanguages
    {
        public AdminLanguages()
        {
            this.LanguageValues = new HashSet<AdminLanguageValues>();
        }
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsDefault { get; set; }
        public virtual ICollection<AdminLanguageValues> LanguageValues { get; set; }
    }
}