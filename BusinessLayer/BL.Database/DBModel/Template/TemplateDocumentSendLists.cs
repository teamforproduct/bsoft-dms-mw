using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Database.DBModel.Template
{
    public class TemplateDocumentSendLists
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public int SendTypeId { get; set; }
        public Nullable<int> TargetPositionId { get; set; }
        public Nullable<int> TargetAgentId { get; set; }
        public string Description { get; set; }
        public Nullable<DateTime> DueDate { get; set; }
        public int OrderNumber { get; set; }
        public int DueDay { get; set; }
        public int AccessLevelId { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        [ForeignKey("DocumentId")]
        public virtual TemplateDocuments Document { get; set; }
    }
}
