using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Database.DBModel.System
{
    public class PropertyValues
    {
        public int Id { get; set; }
        public int PropertyLinkId { get; set; }
        public int RecordId { get; set; }
        [MaxLength(2000)]
        public string ValueString { get; set; }
        public Nullable<DateTime> ValueDate { get; set; }
        public Nullable<double> ValueNumeric { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        [ForeignKey("PropertyLinkId")]
        public virtual PropertyLinks PropertyLink { get; set; }
    }
}
