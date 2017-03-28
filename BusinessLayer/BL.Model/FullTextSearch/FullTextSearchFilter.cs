using System.Collections.Generic;
using BL.Model.Enums;

namespace BL.Model.FullTextSearch
{
    public class FullTextSearchFilter
    {
        public bool IsNotSplitText { get; set; }
        public int? ParentObjectId { get; set; }
        public EnumObjects? ParentObjectType { get; set; }
        public EnumObjects? ObjectType { get; set; }
        public string Module { get; set; }
        public string Feature { get; set; }
        public bool IsOnlyActual { get; set; }
        public List<string> Accesses { get; set; }
        public List<string> Filters { get; set; }
        public int? RowLimit { get; set; }
    }
}