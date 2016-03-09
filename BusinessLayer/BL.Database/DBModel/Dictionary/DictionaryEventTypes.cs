using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Database.DBModel.Dictionary
{
    public class DictionaryEventTypes
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string SourceDescription { get; set; }
        public string TargetDescription { get; set; }
        public int ImportanceEventTypeId { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        [ForeignKey("ImportanceEventTypeId")]
        public virtual DictionaryImportanceEventTypes ImportanceEventType { get; set; }
    }
}
