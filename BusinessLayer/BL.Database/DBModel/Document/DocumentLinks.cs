using BL.Database.DBModel.Dictionary;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Database.DBModel.Document
{
    public class DocumentLinks
    {
        public int Id { get; set; }
        [Index("IX_DocumentParentDocument", 1, IsUnique = true)]
        public int DocumentId { get; set; }
        [Index("IX_DocumentParentDocument", 2, IsUnique = true)]
        [Index("IX_ParentDocumentId", 1)]
        public int ParentDocumentId { get; set; }
        public int LinkTypeId { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        [ForeignKey("DocumentId")]
        public virtual Documents Document { get; set; }
        [ForeignKey("ParentDocumentId")]
        public virtual Documents ParentDocument { get; set; }
        [ForeignKey("LinkTypeId")]
        public virtual DictionaryLinkTypes LinkType { get; set; }
    }
}
