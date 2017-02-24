using BL.Model.Enums;

namespace BL.Model.FullTextSearch
{
    public class FullTextSearchFilter
    {
        public EnumObjects? ParentObjectType { get; set; }
        public EnumObjects? ObjectType { get; set; }

        public string Module { get; set; }

        public string Feature { get; set; }
    }
}