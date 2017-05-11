using System.Collections.Generic;

namespace BL.Model.WebAPI.Filters
{
    public class FilterAspNetClientServers
    {
        public List<int> ClientServerIds { get; set; }
        public List<int> ServerIds { get; set; }
        public List<int> ClientIds { get; set; }
    }
}
