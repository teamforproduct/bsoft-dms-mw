using System.Collections.Generic;

namespace BL.Model.SystemCore
{
    public class FilterSystemAction
    {
        public List<int> Id { get; set; }
        public string Code { get; set; }
        public string ObjectCode { get; set; }
        public bool? IsAvailable { get; set; }
        public List<int> PositionsIdList { get; set; }


    }
}
