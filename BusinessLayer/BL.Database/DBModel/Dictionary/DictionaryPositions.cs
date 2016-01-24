using System;
using System.Collections.Generic;


namespace BL.Database.DBModel.Dictionary
{
    public class DictionaryPositions
    {
        public DictionaryPositions()
        {
            this.ChildPositions = new HashSet<DictionaryPositions>();
            //this.ChiefDepartments = new HashSet<DictionaryDepartments>();
        }

        public int Id { get; set; }
        public Nullable<int> ParentId { get; set; }
        public string Name { get; set; }
        public int DepartmentId { get; set; }
        public Nullable<int> ExecutorAgentId { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }

        public virtual DictionaryPositions ParentPosition { get; set; }
        public virtual ICollection<DictionaryPositions> ChildPositions { get; set; }
        public virtual DictionaryDepartments Department { get; set; }
        public virtual DictionaryAgents ExecutorAgent { get; set; }
        //public virtual ICollection<DictionaryDepartments> ChiefDepartments { get; set; }

    }
}
