using System;

namespace BL.Database.DBModel.Template
{
    public class TemplateDocuments
    {
        //public TemplateDocuments()
        //{
        //    this.SendLists = new HashSet<TemplateDocumentSendLists>();
        //}

        public int Id { get; set; }
        public string Name { get; set; }
        public int IsHard { get; set; }
        public int DocumentDirectionId { get; set; }
        public int DocumentTypeId { get; set; }
        public Nullable<int> DocumentSubjectId { get; set; }
        public string Description { get; set; }
        public Nullable<int> RegistrationJournalId { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }

        public virtual TemplateDocumentIncomingDetails IncomingDetail { get; set; }
//        public virtual ICollection<TemplateDocumentSendLists> SendLists { get; set; }
    }
}
