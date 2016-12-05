using BL.Model.Enums;
using BL.Model.Extensions;
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
        public DateTime? DateFrom { get { return _DateFrom; } set { _DateFrom = value.ToUTC(); } }
        private DateTime? _DateFrom;
        public DateTime? DateTo { get { return _DateTo; } set { _DateTo = value.ToUTC(); } }
        private DateTime? _DateTo;
        public List<int> Ids { get; set; }
    }
}
