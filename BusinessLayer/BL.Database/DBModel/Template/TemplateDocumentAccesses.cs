using BL.Database.DBModel.Admin;
using BL.Database.DBModel.Dictionary;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Database.DBModel.Template
{
    [Table("TemplateDocumentAccesses")]
    public class TemplateDocumentAccesses
    {
        public int Id { get; set; }
        [Index("IX_DocumentPosition", 1, IsUnique = true)]
        public int DocumentId { get; set; }
        [Index("IX_DocumentPosition", 2, IsUnique = true)]
        [Index("IX_PositionId", 1)]
        public Nullable<int> PositionId { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        [ForeignKey("DocumentId")]
        public virtual TemplateDocuments Document { get; set; }
        [ForeignKey("PositionId")]
        public virtual DictionaryPositions Position { get; set; }
    }
}
