using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Database.DBModel.Dictionary
{

    public class DictionaryCompanies
    {

        public DictionaryCompanies()
        {
            this.Departments = new HashSet<DictionaryDepartments>();
        }

        public int Id { get; set; }
        [Index("IX_FullName", 2, IsUnique = true)]
        [Index("IX_ClientId", 1)]
        public int ClientId { get; set; }
        [MaxLength(400)]
        [Index("IX_FullName", 1, IsUnique = true)]
        public string FullName { get; set; }
        [MaxLength(2000)]
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        [ForeignKey("Id")]
        public virtual DictionaryAgents Agent { get; set; }

        // Добавил Departments для выполнения субзапросов от DictionaryCompanies к DictionaryDepartments. В DictionaryDepartments это поле CompanyId
        //[ForeignKey("CompanyId")]
        public virtual ICollection<DictionaryDepartments> Departments { get; set; }
    }

}
