using BL.Model.Common;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BL.Model.WebAPI.Filters
{
    public class FilterAspNetUserFingerprint : BaseFilter
    {
        public List<string> UserIDs { get; set; }
        [IgnoreDataMember]
        public string NameExact { get; set; }

        public string FingerprintExact { get; set; }

        [IgnoreDataMember]
        public string BrowserExact { get; set; }
        [IgnoreDataMember]
        public string PlatformExact { get; set; }
        public bool? IsActive { get; set; }

    }
}
