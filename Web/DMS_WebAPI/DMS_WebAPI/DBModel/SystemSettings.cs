using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMS_WebAPI.DBModel
{
    public class SystemSettings
    {
        public int Id { get; set; }

        [MaxLength(400)]
        [Index("IX_Key", IsUnique = true)]
        public string Key { get; set; }

        [MaxLength(400)]
        public string Name { get; set; }

        [MaxLength(2000)]
        public string Description { get; set; }

        [MaxLength(2000)]
        public string Value { get; set; }
        public int ValueTypeId { get; set; }
        public int Order { get; set; }

        [ForeignKey("ValueTypeId")]
        public virtual SystemValueTypes ValueType { get; set; }
    }
}