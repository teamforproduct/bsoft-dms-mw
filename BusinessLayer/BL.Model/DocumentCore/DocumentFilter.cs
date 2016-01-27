using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DocumentCore
{
    class DocumentFilter
    {
        public List<int> TemplateDocumentId { get; set; }
        public List<int> DocumentDirectionId { get; set; }
        public List<int> DocumentTypeId { get; set; }
        public Nullable<DateTime> CreateFromDate { get; set; }
        public Nullable<DateTime> CreateToDate { get; set; }
        public List<int> DocumentSubjectId { get; set; }
        public string Description { get; set; }
        public List<int> RegistrationJournalId { get; set; }
        public string RegistrationNumber { get; set; }
        public Nullable<DateTime> RegistrationFromDate { get; set; }
        public Nullable<DateTime> RegistrationToDate { get; set; }
        public List<int> ExecutorPositionId { get; set; }
        public List<int> SenderAgentId { get; set; }
        public string SenderPerson { get; set; }
        public string SenderNumber { get; set; }
        public Nullable<DateTime> SenderFromDate { get; set; }
        public Nullable<DateTime> SenderToDate { get; set; }
        public string Addressee { get; set; }

    }
}
