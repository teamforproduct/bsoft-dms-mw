using BL.Model.Enums;
using System.Collections.Generic;
using BL.Model.DocumentCore.InternalModel;

namespace BL.Model.SystemCore.InternalModel
{
    public class InternalSystemAction
    {
        public int Id { get; set; }
        public EnumObjects ObjectId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public EnumActionCategories? Category { get; set; }
        public int? PermissionId { get; set; }
        public IEnumerable<InternalActionRecord> ActionRecords { get; set; }
        
    }
}
