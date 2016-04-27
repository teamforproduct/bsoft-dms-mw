using BL.Model.Common;

namespace BL.Model.SystemCore.InternalModel
{
    public class InternalProperty: LastChangeInfo
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string TypeCode { get; set; }
        public string Description { get; set; }
        public string Label { get; set; }
        public string Hint { get; set; }
        public int? ValueTypeId { get; set; }
        public string OutFormat { get; set; }
        public string InputFormat { get; set; }
        public string SelectAPI { get; set; }
        public string SelectFilter { get; set; }
        public string SelectFieldCode { get; set; }
        public string SelectDescriptionFieldCode { get; set; }

        public string SelectTable { get; set; }

        public InternalSystemValueType ValueType { get; set; }
    }
}