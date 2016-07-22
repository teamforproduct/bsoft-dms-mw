
namespace BL.Database.DBModel.System
{
    using global::System.ComponentModel.DataAnnotations;
    using global::System.ComponentModel.DataAnnotations.Schema;
    using System;

    public class SystemUIElements
    {
        public int Id { get; set; }
        [Index("IX_ActionCode", 1, IsUnique = true)]
        public int ActionId { get; set; }
        [MaxLength(400)]
        [Index("IX_ActionCode", 2, IsUnique = true)]
        public string Code { get; set; }
        [MaxLength(400)]
        public string TypeCode { get; set; }
        [MaxLength(2000)]
        public string Description { get; set; }
        [MaxLength(2000)]
        public string Label { get; set; }
        [MaxLength(2000)]
        public string Hint { get; set; }
        public int ValueTypeId { get; set; }
        public bool IsMandatory { get; set; }
        public bool IsReadOnly { get; set; }
        public bool IsVisible { get; set; }
        [MaxLength(2000)]
        public string SelectAPI { get; set; }
        [MaxLength(2000)]
        public string SelectFilter { get; set; }
        [MaxLength(2000)]
        public string SelectFieldCode { get; set; }
        [MaxLength(2000)]
        public string SelectDescriptionFieldCode { get; set; }
        [MaxLength(2000)]
        public string ValueFieldCode { get; set; }
        [MaxLength(2000)]
        public string ValueDescriptionFieldCode { get; set; }
        [MaxLength(2000)]
        public string Format { get; set; }
        [ForeignKey("ActionId")]
        public virtual SystemActions Action { get; set; }
        [ForeignKey("ValueTypeId")]
        public virtual SystemValueTypes ValueType { get; set; }
    }
}
