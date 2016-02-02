using BL.Model.Enums;
using System;
using System.Collections.Generic;

namespace BL.Model.DictionaryCore
{
    public class BaseDictionaryDocumentSubject
    {
        public int Id { get; set; }
        public Nullable<int> ParentId { get; set; }
        public string Name { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }

        public string ParentDocumentSubjectName { get; set; }

        public virtual IEnumerable<BaseDictionaryDocumentSubject> ChildDocumentSubjects { get; set; }

    }
}