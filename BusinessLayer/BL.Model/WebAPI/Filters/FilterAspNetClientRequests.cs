using System.Collections.Generic;

namespace BL.Model.WebAPI.Filters
{
    public class FilterAspNetClientRequests
    {
        public List<int> IDs { get; set; }
        public string CodeExact { get; set; }
        public string HashCodeExact { get; set; }
        public string SMSCodeExact { get; set; }
    }
}
