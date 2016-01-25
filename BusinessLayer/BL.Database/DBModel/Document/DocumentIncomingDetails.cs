using BL.Database.DBModel.Dictionary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Database.DBModel.Document
{
    public class DocumentIncomingDetails
    {
        public int Id { get; set; }
        public int SenderAgentId { get; set; }
        public string SenderPerson { get; set; }
        public string SenderNumber { get; set; }
        public System.DateTime SenderDate { get; set; }
        public string Addressee { get; set; }
        public int LastChangeUserId { get; set; }
        public System.DateTime LastChangeDate { get; set; }

        public virtual Documents Document { get; set; }
        public virtual DictionaryAgents SenderAgent { get; set; }
    }
}
