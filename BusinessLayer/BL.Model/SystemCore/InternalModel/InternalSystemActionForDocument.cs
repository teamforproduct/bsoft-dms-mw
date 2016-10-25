using BL.Model.Enums;
using System.Collections.Generic;
using BL.Model.DocumentCore.InternalModel;

namespace BL.Model.SystemCore.InternalModel
{
    public class InternalSystemActionForDocument
    {
        public EnumDocumentActions DocumentAction { get; set; }
        public EnumObjects Object { get; set; }
        public string ActionCode { get; set; }
        public string ObjectCode { get; set; }
        public string API { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public IEnumerable<InternalActionRecord> ActionRecords { get; set; }
        
    }
}
