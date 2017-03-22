using BL.Model.Common;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BL.Model.DictionaryCore.FilterModel
{
    public class FilterAgentFavourite : BaseFilter
    {
        [IgnoreDataMember]
        public string ModuleExact { get; set; }

        [IgnoreDataMember]
        public string FeatureExact { get; set; }

        public List<int> AgentIDs { get; set; }

        public List<int> ObjectIDs { get; set; }
    }
}
