using System.Runtime.Serialization;

namespace BL.Model.SystemCore.IncomingModel
{
    public class ModifyProperty
    {
        [IgnoreDataMember]
        public int Id { get; set; }
        public string Code { get; set; }
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
    }
}
