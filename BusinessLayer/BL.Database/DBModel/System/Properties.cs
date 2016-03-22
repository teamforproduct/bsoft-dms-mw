using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Database.DBModel.System
{
    public partial class Properties
    {
        public int Id { get; set; }
        [MaxLength(2000)]
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
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }

        public virtual SystemValueTypes ValueType { get; set; }
    }
}
