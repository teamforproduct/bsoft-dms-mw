using BL.Model.Enums;

namespace BL.Model.FullTextSearch
{
    public class FullTextSearchFilter
    {
        public EnumObjects? ParentObjectType { get; set; }
        public EnumObjects? ObjectType { get; set; }

        public int? ModuleId { get; set; }

        public int? FeatureId { get; set; }
    }
}