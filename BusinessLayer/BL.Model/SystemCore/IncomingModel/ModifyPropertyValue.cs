using System;
using System.Runtime.Serialization;

namespace BL.Model.SystemCore.IncomingModel
{
    public class ModifyPropertyValue
    {
        [IgnoreDataMember]
        public int Id { get; set; }
        public int PropertyLinkId { get; set; }
        public int RecordId { get; set; }
        public string ValueString { get; set; }
        public DateTime? ValueDate { get; set; }
        public double? ValueNumeric { get; set; }
    }
}
