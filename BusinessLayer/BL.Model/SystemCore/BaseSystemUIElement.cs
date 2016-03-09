using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.SystemCore
{
    public class BaseSystemUIElement
    {
        public int Id { get; set; }
        public int ActiontId { get; set; }
        public string ObjectCode { get; set; }
        public string ActionCode { get; set; }
        public string Code { get; set; }
        public string TypeCode { get; set; }
        public string Description { get; set; }
        public string Label { get; set; }
        public string Hint { get; set; }
        public Nullable<int> ValueTypeId { get; set; }
        public string ValueTypeCode { get; set; }
        public bool IsMandatory { get; set; }
        public bool IsReadOnly { get; set; }
        public bool IsVisible { get; set; }
        public string SelectAPI { get; set; }
        public string SelectFilter { get; set; }
        public string SelectFieldCode { get; set; }
        public string SelectDescriptionFieldCode { get; set; }
        public string ValueFieldCode { get; set; }
        public string ValueDescriptionFieldCode { get; set; }
        public string Format { get; set; }
        public int? PropertyLinkId { get; set; }
    }
}
