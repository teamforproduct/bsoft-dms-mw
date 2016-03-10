using BL.Model.Enums;
using System.Collections.Generic;

namespace BL.Model.SystemCore.Filters
{
    public class FilterPropertyLink
    {
        public List<int> PropertyLinkId { get; set; }
        public List<EnumObjects> Object { get; set; }
    }
}
