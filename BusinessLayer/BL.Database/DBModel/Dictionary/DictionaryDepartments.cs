using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Database.DBModel.Dictionary
{

    public class DictionaryDepartments
    {
        public DictionaryDepartments()
        {
            this.ChildDepartments = new HashSet<DictionaryDepartments>();
        }

        public int Id { get; set; }
        public Nullable<int> ParentId { get; set; }
        public int CompanyId { get; set; }
        public string Name { get; set; }
        public int? ChiefPositionId { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        [ForeignKey("ParentId")]
        public virtual DictionaryDepartments ParentDepartment { get; set; }
        [ForeignKey("CompanyId")]
        public virtual DictionaryCompanies Company { get; set; }
        [ForeignKey("ChiefPositionId")]
        public virtual DictionaryPositions ChiefPosition { get; set; }

        public virtual ICollection<DictionaryDepartments> ChildDepartments { get; set; }
        //public virtual DictionaryPositions ChiefPosition { get; set; }
    }
}
