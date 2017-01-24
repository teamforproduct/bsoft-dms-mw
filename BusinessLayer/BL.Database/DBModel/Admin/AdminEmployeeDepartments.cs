using BL.Database.DBModel.Dictionary;
using System;
using System.ComponentModel.DataAnnotations.Schema;


namespace BL.Database.DBModel.Admin
{
    public class AdminEmployeeDepartments
    {
        public int Id { get; set; }

        [Index("IX_EmployeeDepartment", 1, IsUnique = true)]
        [Index("IX_EmployeeId", 1)]
        public int EmployeeId { get; set; }


        [Index("IX_EmployeeDepartment", 2, IsUnique = true)]
        [Index("IX_DepartmentId", 1)]
        public int DepartmentId { get; set; }

        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }


        [ForeignKey("EmployeeId")]
        public virtual DictionaryAgentEmployees Employee { get; set; }

        [ForeignKey("DepartmentId")]
        public virtual DictionaryDepartments Department { get; set; }
    }
}
