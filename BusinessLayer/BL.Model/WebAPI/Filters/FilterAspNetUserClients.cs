using BL.Model.Enums;
using System.Collections.Generic;

namespace BL.Model.WebAPI.Filters
{
    public class FilterAspNetUserClients
    {
        public List<int> UserClientIds { get; set; }
        public List<string> UserIds { get; set; }
        public List<int> ClientIds { get; set; }
        public string ClientCode { get; set; }
    }
}
