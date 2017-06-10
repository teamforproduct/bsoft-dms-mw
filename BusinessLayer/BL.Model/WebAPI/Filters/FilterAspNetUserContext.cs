using BL.Model.Common;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BL.Model.WebAPI.Filters
{
    public class FilterAspNetUserContext : BaseFilter
    {
        public List<string> UserIDs { get; set; }

        [IgnoreDataMember]
        public string Key { get; set; }

        [IgnoreDataMember]
        public List<int> ClientIDs { get; set; }

        [IgnoreDataMember]
        public DateTime? LastUsegeDateLess { get; set; }
    }
}
