using BL.Model.Database;
using System.Collections.Generic;

namespace BL.Model.WebAPI.Filters
{
    public class FilterAspNetClients
    {
        public List<int> ClientIds { get; set; }
        public string Code { get; set; }
        public string VerificationCode { get; set; }
    }
}
