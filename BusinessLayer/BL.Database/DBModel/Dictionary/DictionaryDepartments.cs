using System;
using System.Collections.Generic;

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
        public int ChiefPositionId { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }

        public virtual DictionaryDepartments ParentDepartment { get; set; }
        public virtual ICollection<DictionaryDepartments> ChildDepartments { get; set; }
        public virtual DictionaryCompanies Company { get; set; }
        //public virtual DictionaryPositions ChiefPosition { get; set; }
    }
}
