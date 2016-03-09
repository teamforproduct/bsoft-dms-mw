using BL.Model.Common;
using System;

namespace BL.Model.SystemCore.InternalModel
{
    public class InternalPropertyValue : LastChangeInfo
    {
        public int Id { get; set; }
        public int PropertyLinkId { get; set; }
        public int RecordId { get; set; }
        public string ValueString { get; set; }
        public DateTime? ValueDate { get; set; }
        public double? ValueNumeric { get; set; }

        public InternalPropertyLink PropertyLink { get; set; }
    }
}