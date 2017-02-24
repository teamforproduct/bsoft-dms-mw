﻿using BL.Model.Common;
using System.Collections.Generic;

namespace BL.Model.DictionaryCore.FilterModel
{
    public class FilterAgentFavourite : BaseFilter
    {
        public string ModuleExact { get; set; }

        public string FeatureExact { get; set; }

        public List<int> AgentIDs { get; set; }

        public List<int> ObjectIDs { get; set; }
    }
}