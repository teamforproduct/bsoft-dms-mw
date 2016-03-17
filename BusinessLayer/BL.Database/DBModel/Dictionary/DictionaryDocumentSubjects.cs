using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

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
        public bool IsActive { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        [ForeignKey("ParentId")]
        public virtual DictionaryDocumentSubjects ParentDocumentSubject { get; set; }
        public virtual ICollection<DictionaryDocumentSubjects> ChildDocumentSubjects { get; set; }
    }
}
