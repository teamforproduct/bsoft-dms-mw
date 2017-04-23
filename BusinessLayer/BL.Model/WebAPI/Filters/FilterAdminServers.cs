using BL.Model.Enums;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BL.Model.WebAPI.Filters
{
    public class FilterAdminServers
    {
        public List<int> ServerIDs { get; set; }
        public List<int> ClientIDs { get; set; }
        public List<EnumDatabaseType> ServerTypes { get; set; }
        public string ClientCode { get; set; }

        [IgnoreDataMember]
        public string ServerNameExact { get; set; }
    }
}
