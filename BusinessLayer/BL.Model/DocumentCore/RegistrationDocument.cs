using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DocumentCore
{
    public class RegistrationDocument
    {
        public int Id { get; set; }
        public Nullable<int> RegistrationJournalId { get; set; }
        public Nullable<int> RegistrationNumber { get; set; }
        public string RegistrationNumberSuffix { get; set; }
        public string RegistrationNumberPrefix { get; set; }
        public Nullable<DateTime> RegistrationDate { get; set; }
        public bool IsSign { get; set; }
        public bool IsSendDocument { get; set; }
        public bool IsSendEmeil { get; set; }        
    }
}
