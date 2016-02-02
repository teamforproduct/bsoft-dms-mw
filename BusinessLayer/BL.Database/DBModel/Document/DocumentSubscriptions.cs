using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Database.DBModel.Document
{
    public class DocumentSubscriptions
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public int SendEventId { get; set; }
        public Nullable<int> DoneEventId { get; set; }
        public string Description { get; set; }
        public string Hash { get; set; }
        public string ChangedHash { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        [ForeignKey("DocumentId")]
        public virtual Documents Document { get; set; }
        [ForeignKey("SendEventId")]
        public virtual DocumentEvents SendEvent { get; set; }
        [ForeignKey("DoneEventId")]
        public virtual DocumentEvents DoneEvent { get; set; }
    }
}
