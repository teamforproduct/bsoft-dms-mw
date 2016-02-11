using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Database.DBModel.Dictionary
{
    public class DictionarySendTypes
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsImportant { get; set; }
        public int SubordinationTypeId { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        [ForeignKey("SubordinationTypeId")]
        public virtual DictionarySubordinationTypes SubordinationType { get; set; }
    }
}
