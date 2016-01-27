using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Database.DBModel.Dictionary
{
    public class DictionaryPositions
    {
        public DictionaryPositions()
        {
            this.ChildPositions = new HashSet<DictionaryPositions>();
            //this.Subordinations = new HashSet<AdminSubordination>();
            //this.AddresseeSubordinations = new HashSet<AdminSubordination>();
            //this.Settings = new HashSet<AdminSettings>();
            this.ChiefDepartments = new HashSet<DictionaryDepartments>();
            this.StandartSendLists = new HashSet<DictionaryStandartSendLists>();
        }

        public int Id { get; set; }
        public Nullable<int> ParentId { get; set; }
        public string Name { get; set; }
        public int DepartmentId { get; set; }
        public Nullable<int> ExecutorAgentId { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        [ForeignKey("ParentId")]
        public virtual DictionaryPositions ParentPosition { get; set; }
        [ForeignKey("DepartmentId")]
        public virtual DictionaryDepartments Department { get; set; }
        [ForeignKey("ExecutorAgentId")]
        public virtual DictionaryAgents ExecutorAgent { get; set; }
        //public virtual ICollection<AdminSubordination> Subordinations { get; set; }
        //public virtual ICollection<AdminSubordination> AddresseeSubordinations { get; set; }
        //public virtual ICollection<AdminSettings> Settings { get; set; }
        public virtual ICollection<DictionaryPositions> ChildPositions { get; set; }
        public virtual ICollection<DictionaryDepartments> ChiefDepartments { get; set; }
        public virtual ICollection<DictionaryStandartSendLists> StandartSendLists { get; set; }

    }
}
