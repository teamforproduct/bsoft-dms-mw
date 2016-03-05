using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Database.DBModel.System
{
    public partial class Properties
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Label { get; set; }
        public string Hint { get; set; }
        public Nullable<int> ValueTypeId { get; set; }
        public string OutFormat { get; set; }
        public string InputFormat { get; set; }
        public string SelectAPI { get; set; }
        public string SelectFilter { get; set; }
        public string SelectFieldCode { get; set; }
        public string SelectDescriptionFieldCode { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }

        public virtual SystemValueTypes ValueType { get; set; }
    }
}
