using System;

namespace BL.Model.DocumentCore.Actions
{
    public class RegisterDocument
    {
        public int DocumentId { get; set; }
        public int RegistrationJournalId { get; set; }
        public bool IsOnlyGetNextNumber { get; set; }
        public int? RegistrationNumber { get; set; }
        public string RegistrationNumberSuffix { get; set; }
        public string RegistrationNumberPrefix { get; set; }
        public DateTime RegistrationDate { get; set; }    
    }
}
