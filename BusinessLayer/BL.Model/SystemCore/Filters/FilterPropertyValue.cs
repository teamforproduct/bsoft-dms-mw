using BL.Model.Enums;
using System.Collections.Generic;

namespace BL.Model.SystemCore.Filters
{
    public class FilterPropertyValue
    {
        public List<int> PropertyValuesId { get; set; }
        public List<int> RecordId { get; set; }
        public List<EnumObjects> Object { get; set; }
    }
}
