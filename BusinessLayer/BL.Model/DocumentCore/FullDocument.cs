using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DocumentCore
{
    public class FullDocument :BaseDocument
    {
        public string TemplateDocumentName { get; set; }
        public int IsHard { get; set; }
        public string DocumentDirectionName { get; set; }
        public string DocumentTypeName { get; set; }
        public DateTime CreateDate { get; set; }
        public string DocumentSubjectName { get; set; }
        public int? RegistrationJournalId { get; set; }
        public string RegistrationJournalName { get; set; }
        public int? RegistrationNumber { get; set; }
        public string RegistrationNumberSuffix { get; set; }
        public string RegistrationNumberPrefix { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public string ExecutorPositionName { get; set; }
        public string ExecutorAgentName { get; set; }
        public string RestrictedSendListName { get; set; }
        public string SenderAgentName { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
    }
}
