using BL.Model.Database;
using System.Collections.Generic;

namespace BL.Model.WebAPI.Filters
{
    public class FilterAdminServers
    {
        public List<int> ServerIds { get; set; }
        public List<DatabaseType> ServerTypes { get; set; }
    }
}
