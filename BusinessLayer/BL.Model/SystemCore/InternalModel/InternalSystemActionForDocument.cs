using BL.Model.Enums;
using System.Collections.Generic;
using BL.Model.DocumentCore.InternalModel;

namespace BL.Model.SystemCore.InternalModel
{
    public class InternalSystemActionForDocument
    {
        public EnumActions DocumentAction { get; set; }
        public EnumObjects Object { get; set; }
        public string ActionCode { get; set; }
        public string ObjectCode { get; set; }
        public string Description { get; set; }
        public EnumActionCategories? Category { get; set; }
        //public string CategoryName { get; set; }
        public List<InternalActionRecord> ActionRecords { get; set; }
        
    }
}
