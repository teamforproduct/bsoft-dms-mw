using System;

namespace BL.Model.DocumentCore.Actions
{
    public class RegistrationDocument
    {
        public int Id { get; set; }
        public int RegistrationJournalId { get; set; }
        public int? RegistrationNumber { get; set; }
        public string RegistrationNumberSuffix { get; set; }
        public string RegistrationNumberPrefix { get; set; }
        public DateTime RegistrationDate { get; set; }    
    }
}
