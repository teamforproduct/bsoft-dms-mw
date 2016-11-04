using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DMS_WebAPI.DBModel
{
    public class AdminLanguages
    {
        public AdminLanguages()
        {
            this.LanguageValues = new HashSet<AdminLanguageValues>();
        }
        /// <summary>
        /// Цифровой код языка
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Язык ISO 639-1
        /// </summary>
        [MaxLength(2000)]
        public string Code { get; set; }
        
        /// <summary>
        /// Язык
        /// </summary>
        [MaxLength(2000)]
        public string Name { get; set; }
        public bool IsDefault { get; set; }
        public virtual ICollection<AdminLanguageValues> LanguageValues { get; set; }
    }
}