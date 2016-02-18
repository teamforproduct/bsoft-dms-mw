using System.Collections.Generic;

namespace BL.Model.DictionaryCore
{
    public class FilterDictionaryDepartment
    {
        public List<int> Id { get; set; }
        public List<int> ParentId { get; set; }
        public int? CompanyId { get; set; }
        public string Name { get; set; }
        public int? ChiefPositionId { get; set; }

    }
}
