using BL.Model.Enums;
using System.Collections.Generic;

namespace BL.Model.SystemCore.Filters
{
    public class FilterSystemAction
    {
        public List<int> IDs { get; set; }
        public string Description { get; set; }
        public EnumDocumentActions? DocumentAction { get; set; }
        public EnumObjects? ObjectId { get; set; }
        public bool? IsAvailable { get; set; }
        public List<int> PositionsIDs { get; set; }
    }
}
