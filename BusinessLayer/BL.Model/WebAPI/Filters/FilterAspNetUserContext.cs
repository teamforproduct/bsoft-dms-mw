using BL.Model.Common;
using System.Collections.Generic;

namespace BL.Model.WebAPI.Filters
{
    public class FilterAspNetUserContext : BaseFilter
    {
        public List<string> UserIDs { get; set; }

        public string TokenExact { get; set; }

    }
}
