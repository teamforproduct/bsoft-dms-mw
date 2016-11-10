using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Database.DBModel.Dictionary
{

    public class DictionaryDepartments
    {
        public DictionaryDepartments()
        {
            this.ChildDepartments = new HashSet<DictionaryDepartments>();
            this.Positions = new HashSet<DictionaryPositions>();
        }

        public int Id { get; set; }
        [Index("IX_CompanyParentName", 2, IsUnique = false)]
        [Index("IX_ParentId", 1)]
        public Nullable<int> ParentId { get; set; }
        [Index("IX_CompanyParentName", 1, IsUnique = false)]
        public int CompanyId { get; set; }
        [MaxLength(2000)]
        public string Code { get; set; }
        [MaxLength(400)]
        [Index("IX_CompanyParentName", 3, IsUnique = false)]
        public string Name { get; set; }
        [MaxLength(2000)]
        public string FullName { get; set; }
        [MaxLength(2000)]
        public string FullPath { get; set; }
        public int? ChiefPositionId { get; set; }
        public bool IsActive { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }

        [ForeignKey("ParentId")]
        public virtual DictionaryDepartments ParentDepartment { get; set; }
        [ForeignKey("CompanyId")]
        public virtual DictionaryCompanies Company { get; set; }
        [ForeignKey("ChiefPositionId")]
        public virtual DictionaryPositions ChiefPosition { get; set; }

        public virtual ICollection<DictionaryDepartments> ChildDepartments { get; set; }
        // Добавил Positions для выполнения субзапросов от DictionaryDepartments к DictionaryPositions. В DictionaryPositions это поле DepartmentId
        [ForeignKey("DepartmentId")]
        public virtual ICollection<DictionaryPositions> Positions { get; set; }
        //public virtual DictionaryPositions ChiefPosition { get; set; }

        //[ForeignKey("DepartmentId")]
        public virtual ICollection<DictionaryRegistrationJournals> RegistrationJournals { get; set; }

    }
}
