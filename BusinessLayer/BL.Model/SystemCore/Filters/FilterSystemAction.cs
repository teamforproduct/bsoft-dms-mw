using BL.Model.Enums;
using System.Collections.Generic;

namespace BL.Model.SystemCore.Filters
{
    public class FilterSystemAction
    {
        public List<int> ActionId { get; set; }
        public EnumDocumentActions? DocumentAction { get; set; }
        public EnumObjects? Object { get; set; }
        public bool? IsAvailable { get; set; }
        public List<int> PositionsIdList { get; set; }


    }
}
