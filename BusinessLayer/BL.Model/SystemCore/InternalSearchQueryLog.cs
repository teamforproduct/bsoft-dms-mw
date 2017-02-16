using System;
using BL.Model.Enums;
using BL.Model.Common;

namespace BL.Model.SystemCore
{
    public class InternalSearchQueryLog: LastChangeInfo
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int ModuleId { get; set; }
        public int FeatureId { get; set; }
        public string SearchQueryText { get; set; }
    }
}