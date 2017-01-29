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
        public string API { get; set; }
        public string Description { get; set; }
        public bool IsGrantable { get; set; }
        public bool IsGrantableByRecordId { get; set; }
        public bool IsVisible { get; set; }
        public bool IsVisibleInMenu { get; set; }
        public int? GrantId { get; set; }
        public string Category { get; set; }
        public int? PermissionId { get; set; }
        public IEnumerable<InternalActionRecord> ActionRecords { get; set; }
        
    }
}
