using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Database.DBModel.Dictionary;

namespace BL.Database.DBModel.Template
{
    public partial class TemplateDocumentTasks
    {
        public TemplateDocumentTasks()
        {
            this.SendLists = new HashSet<TemplateDocumentSendLists>();
        }

        public int Id { get; set; }
        public int DocumentId { get; set; }
        public Nullable<int> PositionId { get; set; }
        public string Task { get; set; }
        public string Description { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        [ForeignKey("DocumentId")]
        public virtual TemplateDocuments Document { get; set; }
        [ForeignKey("PositionId")]
        public virtual DictionaryPositions Position { get; set; }
        public virtual ICollection<TemplateDocumentSendLists> SendLists { get; set; }
    }
}
