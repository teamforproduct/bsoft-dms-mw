using BL.Model.DictionaryCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DocumentCore
{
    public class BaseDocumentTemporaryRegistration
    {
        public int Id { get; set; }
        public int RegistrationJournalId { get; set; }
        public int RegistrationNumber { get; set; }
        public string RegistrationNumberSuffix { get; set; }
        public string RegistrationNumberPrefix { get; set; }
        public DateTime RegistrationDate { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        public string RegistrationJournalName { get; set; }
    }
}
