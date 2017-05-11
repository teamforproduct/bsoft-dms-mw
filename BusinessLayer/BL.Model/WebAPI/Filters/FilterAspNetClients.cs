using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BL.Model.WebAPI.Filters
{
    public class FilterAspNetClients
    {
        [IgnoreDataMember]
        public List<int> IDs { get; set; }
        public string Code { get; set; }
        [IgnoreDataMember]
        public string VerificationCode { get; set; }
    }
}
