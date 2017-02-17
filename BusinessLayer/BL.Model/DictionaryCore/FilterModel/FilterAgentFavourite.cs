using BL.Model.Common;
using System.Collections.Generic;

namespace BL.Model.DictionaryCore.FilterModel
{
    public class FilterAgentFavourite : BaseFilter
    {
        public string Module { get; set; }

        public string Feature { get; set; }

        public List<int> AgentIDs { get; set; }
    }
}
