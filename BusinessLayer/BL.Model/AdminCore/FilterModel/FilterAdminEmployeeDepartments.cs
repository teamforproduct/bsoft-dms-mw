using BL.Model.Common;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BL.Model.AdminCore.FilterModel
{
    /// <summary>
    /// Фильтры FilterAdminSubordination
    /// </summary>
    public class FilterAdminEmployeeDepartments : BaseFilter
    {
        [IgnoreDataMember]
        public List<int> DepartmentIDs { get; set; }
        [IgnoreDataMember]
        public List<int> EmployeeIDs { get; set; }

    }
}