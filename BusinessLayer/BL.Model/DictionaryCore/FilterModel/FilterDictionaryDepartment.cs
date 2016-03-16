using System.Collections.Generic;

namespace BL.Model.DictionaryCore.FilterModel
{
    public class FilterDictionaryDepartment
    {
        public List<int> DepartmentId { get; set; }
        public List<int> ParentId { get; set; }
        public int? CompanyId { get; set; }
        public string Name { get; set; }
        public int? ChiefPositionId { get; set; }
        public List<int> NotContainsId { get; set; }
    }
}
