using System.Collections.Generic;

namespace BL.Model.WebAPI.Filters
{
    public class FilterAspNetClientServer
    {
        public List<int> IDs { get; set; }
        public List<int> ServerIDs { get; set; }
        public List<int> ClientIDs { get; set; }
    }
}
