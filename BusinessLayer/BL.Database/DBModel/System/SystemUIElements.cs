
namespace BL.Database.DBModel.System
{
    using global::System.ComponentModel.DataAnnotations;
    using global::System.ComponentModel.DataAnnotations.Schema;

    public class SystemUIElements
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public int ActionId { get; set; }

        [MaxLength(400)]
        public string Code { get; set; }

        [MaxLength(400)]
        public string TypeCode { get; set; }

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
        public int Order { get; set; }
        [ForeignKey("ActionId")]
        public virtual SystemActions Action { get; set; }
        [ForeignKey("ValueTypeId")]
        public virtual SystemValueTypes ValueType { get; set; }
    }
}
