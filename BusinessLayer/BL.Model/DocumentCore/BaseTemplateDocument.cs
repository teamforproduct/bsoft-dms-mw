using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DocumentCore
{
    public class BaseTemplateDocument
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int IsHard { get; set; }
        public int DocumentDirectionId { get; set; }
        public int DocumentTypeId { get; set; }
        public Nullable<int> DocumentSubjectId { get; set; }
        public string Description { get; set; }
        public Nullable<int> RegistrationJournalId { get; set; }
        public Nullable<int> RestrictedSendListId { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
    }
}
