using System;
using System.Collections.Generic;

namespace BL.Database.DBModel.Dictionary
{
    public class DictionaryDocumentSubjects
    {
        public DictionaryDocumentSubjects()
        {
            this.ChildDocumentSubjects = new HashSet<DictionaryDocumentSubjects>();
        }

        public int Id { get; set; }
        public Nullable<int> ParentId { get; set; }
        public string Name { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }

        public virtual DictionaryDocumentSubjects ParentDocumentSubject { get; set; }
        public virtual ICollection<DictionaryDocumentSubjects> ChildDocumentSubjects { get; set; }
    }
}
