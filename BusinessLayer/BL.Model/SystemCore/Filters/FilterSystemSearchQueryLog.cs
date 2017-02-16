using System;
using BL.Model.Enums;
using BL.Model.Common;
using System.Collections.Generic;

namespace BL.Model.SystemCore.Filters
{
    public class FilterSystemSearchQueryLog : BaseFilter
    {
        public List<int> ClientId { get; set; }
        public List<int> ModuleId { get; set; }
        public List<int> FeatureId { get; set; }
        public string AllSearchQueryTextParts { get; set; }
        public string OneSearchQueryTextParts { get; set; }
    }
}