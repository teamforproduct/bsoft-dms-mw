using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Database.DBModel.Template
{
    public class TemplateDocuments
    {
        //public TemplateDocuments()
        //{
        //    this.SendLists = new HashSet<TemplateDocumentSendLists>();
        //}

        public int Id { get; set; }
        public int IsHard { get; set; }
        public int DocumentSourceId { get; set; }
        public int DocumentTypeId { get; set; }
        public Nullable<int> DocumentSubjectId { get; set; }
        public string Description { get; set; }
        public Nullable<int> RegistrationJournalId { get; set; }
        public Nullable<int> ExecutorPositionId { get; set; }
        public int LastChangeUserId { get; set; }
        public System.DateTime LastChangeDate { get; set; }

        public virtual TemplateDocumentIncomingDetails IncomingDetail { get; set; }
//        public virtual ICollection<TemplateDocumentSendLists> SendLists { get; set; }
    }
}
