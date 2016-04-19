using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Database.DBModel.Dictionary
{
    public class DictionaryEventTypes
    {
        public int Id { get; set; }
        [MaxLength(2000)]
        public string Code { get; set; }
        [MaxLength(2000)]
        [Index("IX_Name", 1, IsUnique = true)]
        public string Name { get; set; }
        [MaxLength(2000)]
        public string SourceDescription { get; set; }
        [MaxLength(2000)]
        public string TargetDescription { get; set; }
        public int ImportanceEventTypeId { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        [ForeignKey("ImportanceEventTypeId")]
        public virtual DictionaryImportanceEventTypes ImportanceEventType { get; set; }
    }
}
