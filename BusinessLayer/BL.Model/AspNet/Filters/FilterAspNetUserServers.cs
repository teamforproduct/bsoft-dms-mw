using BL.Model.Enums;
using System.Collections.Generic;

namespace BL.Model.AspNet.Filters
{
    public class FilterAspNetUserServers
    {
        public List<string> UserIds { get; set; }
        public List<int> ServerIds { get; set; }
    }
}
