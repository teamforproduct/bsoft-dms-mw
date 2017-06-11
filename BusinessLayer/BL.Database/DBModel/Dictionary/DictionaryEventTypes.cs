using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Database.DBModel.Dictionary
{
    public class DictionaryEventTypes
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [MaxLength(400)]
        public string Code { get; set; }

        [MaxLength(2000)]
        public string SourceDescription { get; set; }

        [MaxLength(2000)]
        public string TargetDescription { get; set; }

        [MaxLength(2000)]
        public string WaitDescription { get; set; }
        public int ImportanceEventTypeId { get; set; }


        [ForeignKey("ImportanceEventTypeId")]
        public virtual DictionaryImportanceEventTypes ImportanceEventType { get; set; }
    }
}
