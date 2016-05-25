using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Database.DBModel.System
{
    public partial class Properties
    {
        public Properties()
        {
            this.Links = new HashSet<PropertyLinks>();
        }

        public int Id { get; set; }
        [Index("IX_Code", 2, IsUnique = true)]
        [Index("IX_ClientId", 1)]
        public int ClientId { get; set; }
        [MaxLength(2000)]
        [Index("IX_Code", 1, IsUnique = true)]
        public string Code { get; set; }
        [MaxLength(2000)]
        public string TypeCode { get; set; }
        [MaxLength(2000)]
        public string Description { get; set; }
        [MaxLength(2000)]
        public string Label { get; set; }
        [MaxLength(2000)]
        public string Hint { get; set; }
        public Nullable<int> ValueTypeId { get; set; }
        [MaxLength(2000)]
        public string OutFormat { get; set; }
        [MaxLength(2000)]
        public string InputFormat { get; set; }
        [MaxLength(2000)]
        public string SelectAPI { get; set; }
        [MaxLength(2000)]
        public string SelectFilter { get; set; }
        [MaxLength(2000)]
        public string SelectFieldCode { get; set; }
        [MaxLength(2000)]
        public string SelectDescriptionFieldCode { get; set; }
        [MaxLength(2000)]
        public string SelectTable { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }

        public virtual SystemValueTypes ValueType { get; set; }

        [ForeignKey("PropertyId")]
        public virtual ICollection<PropertyLinks> Links { get; set; }
    }
}
