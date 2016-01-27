using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Database.DBModel.Dictionary
{
    public class DictionaryEventTypes
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ImpotanceEventTypeId { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        [ForeignKey("ImpotanceEventTypeId")]
        public virtual DictionaryImpotanceEventTypes ImpotanceEventType { get; set; }
    }
}
