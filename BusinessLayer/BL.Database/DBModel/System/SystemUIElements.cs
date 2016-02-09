using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Database.DBModel.System
{
    using global::System.ComponentModel.DataAnnotations.Schema;
    using System;

    public class SystemUIElements
    {
        public int Id { get; set; }
        public int ActionId { get; set; }
        public string Code { get; set; }
        public string TypeCode { get; set; }
        public string Description { get; set; }
        public string Lable { get; set; }
        public string Hint { get; set; }
        public int ValueTypeId { get; set; }
        public bool IsMandatory { get; set; }
        public bool IsReadOnly { get; set; }
        public bool IsVisible { get; set; }
        public string SelectAPI { get; set; }
        public string SelectFiler { get; set; }
        public string SelectFieldCode { get; set; }
        public string SelectDescriptionFieldCode { get; set; }
        public string ValueFieldCode { get; set; }
        public string ValueDescriptionFieldCode { get; set; }
        public string Format { get; set; }
        [ForeignKey("ActionId")]
        public virtual SystemActions Action { get; set; }
        [ForeignKey("ValueTypeId")]
        public virtual SystemValueTypes ValueType { get; set; }
    }
}
