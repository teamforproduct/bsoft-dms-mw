using System;
using System.Collections.Generic;

namespace BL.Model.DictionaryCore
{
    public class BaseDictionaryDepartment
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public int CompanyId { get; set; }
        public string Name { get; set; }
        public int? ChiefPositionId { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }

        public string ParentDepartmentName { get; set; }
        public string CompanyName { get; set; }
        public string ChiefPositionName { get; set; }

        public virtual IEnumerable<BaseDictionaryDepartment> ChildDepartments { get; set; }
    }
}