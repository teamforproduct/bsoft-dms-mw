using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Database.DBModel.Dictionary
{
    public class CustomDictionaries
    {
        public int Id { get; set; }
        [Index("IX_DictionaryTypeCode", 1, IsUnique = true)]
        public int DictionaryTypeId { get; set; }
        [MaxLength(400)]
        [Index("IX_DictionaryTypeCode", 2, IsUnique = true)]
        public string Code { get; set; }
        [MaxLength(2000)]
        public string Name { get; set; }
        [MaxLength(2000)]
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        [ForeignKey("DictionaryTypeId")]
        public virtual CustomDictionaryTypes CustomDictionaryType { get; set; }
    }
}
