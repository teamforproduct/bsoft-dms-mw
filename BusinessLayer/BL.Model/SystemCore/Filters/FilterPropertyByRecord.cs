using BL.Model.Enums;
using System;
using System.Collections.Generic;

namespace BL.Model.SystemCore.Filters
{
    public class FilterPropertyByRecord
    {
        public int PropertyLinkId { get; set; }
        public EnumValueTypes ValueType { get;set;}
        public string Text { get; set; }
        public double? NumberFrom { get; set; }
        public double? NumberTo { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public List<int> Ids { get; set; }
    }
}
