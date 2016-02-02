using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
