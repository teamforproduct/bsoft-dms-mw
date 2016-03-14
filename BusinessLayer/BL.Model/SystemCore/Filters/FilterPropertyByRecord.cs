using BL.Model.Enums;
using System;
using System.Collections.Generic;

namespace BL.Model.SystemCore.Filters
{
    public class FilterPropertyByRecord
    {
        public int PropertyId { get; set; }
        public EnumValueTypes ValueType { get;set;}
        public string Text { get; set; }
        public int? NumberFrom { get; set; }
        public int? NumberTo { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public List<int> Ids { get; set; }
    }
}
