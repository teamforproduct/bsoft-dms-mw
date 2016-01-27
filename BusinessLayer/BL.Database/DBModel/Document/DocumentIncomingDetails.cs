using BL.Database.DBModel.Dictionary;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Database.DBModel.Document
{
    public class DocumentIncomingDetails
    {
        public int Id { get; set; }
        //public int DocumentId { get; set; }
        public int SenderAgentId { get; set; }
        public string SenderPerson { get; set; }
        public string SenderNumber { get; set; }
        public DateTime SenderDate { get; set; }
        public string Addressee { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
//        [ForeignKey("Id")]
//        public virtual Documents Document { get; set; }
        //public virtual DictionaryAgents SenderAgent { get; set; }
    }
}
