using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DocumentCore
{
    public class ModifyDocumentSendList
    {
        public int DocumentId { get; set; }
        public int OrderNumber { get; set; }
        public int SendTypeId { get; set; }
        public Nullable<int> TargetPositionId { get; set; }
        public string Description { get; set; }
        public Nullable<DateTime> DueDate { get; set; }
        public int DueDay { get; set; }
        public int AccessLevelId { get; set; }
    }
}
