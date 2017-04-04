using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Database.DBModel.Dictionary
{
    public class DictionarySendTypes
    {
        public int Id { get; set; }
        [MaxLength(2000)]
        public string Code { get; set; }
        [MaxLength(2000)]
        [Index("IX_Name", 1, IsUnique = true)]
        public string Name { get; set; }

        public int Order { get; set; }
        public bool IsImportant { get; set; }
        public int SubordinationTypeId { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        [ForeignKey("SubordinationTypeId")]
        public virtual DictionarySubordinationTypes SubordinationType { get; set; }
    }
}
