using BL.Model.Common;
using BL.Model.Enums;
using System.Collections.Generic;

namespace BL.Model.WebAPI.Filters
{
    public class FilterAspNetUserFingerprint : BaseFilter
    {
        public List<string> UserIDs { get; set; }
        public string NameExact { get; set; }

        public string FingerprintExact { get; set; }

        public string BrowserExact { get; set; }
        public string PlatformExact { get; set; }
        public bool? IsActive { get; set; }

    }
}
