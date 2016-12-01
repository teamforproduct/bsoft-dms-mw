using BL.Model.Enums;
using System.Collections.Generic;

namespace BL.Model.WebAPI.Filters
{
    public class FilterAdminServers
    {
        public List<int> ServerIds { get; set; }
        public List<int> ClientIds { get; set; }
        public List<EnumDatabaseType> ServerTypes { get; set; }
    }
}
