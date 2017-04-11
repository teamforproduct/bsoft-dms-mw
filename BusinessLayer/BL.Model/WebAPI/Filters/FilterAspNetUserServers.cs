using BL.Model.Enums;
using System.Collections.Generic;

namespace BL.Model.WebAPI.Filters
{
    public class FilterAspNetUserServers
    {
        public List<int> UserServerIds { get; set; }
        public List<string> UserIds { get; set; }
        public List<int> ServerIds { get; set; }
        public List<int> ClientIDs { get; set; }
    }
}
