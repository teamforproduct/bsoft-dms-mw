using BL.Database.DBModel.Dictionary;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Database.DBModel.Template
{
    public class TemplateDocumentIncomingDetails
    {
        public int Id { get; set; }
        public Nullable<int> SenderAgentId { get; set; }
        public string SenderPerson { get; set; }
        public string SenderNumber { get; set; }
        public Nullable<DateTime> SenderDate { get; set; }
        public string Addressee { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        //        public int DocumentTemplateId { get; set; }
        [ForeignKey("Id")]
        public virtual TemplateDocuments Document { get; set; }
        [ForeignKey("SenderAgentId")]
        public virtual DictionaryAgents SenderAgent { get; set; }
    }
}
