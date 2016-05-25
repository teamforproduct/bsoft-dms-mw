using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DocumentCore.FrontModel
{
    public class FrontDocumentPaperEvent : FrontDocumentInfo
    {
        public int Id { get; set; }

        public int? EventType { get; set; }
        public string EventTypeName { get; set; }
        public DateTime? PlanDate { get; set; }
        public DateTime? SendDate { get; set; }
        public DateTime? RecieveDate { get; set; }


        public string Description { get; set; }






        public string SourceAgentName { get; set; }
        public string SourcePositionExecutorAgentName { get; set; }
        public string SourcePositionExecutorNowAgentName { get; set; }
        public string SourcePositionName { get; set; }
        public string SourcePositionExecutorAgentPhoneNumber { get; set; }

        public string TargetAgentName { get; set; }
        public string TargetPositionExecutorAgentName { get; set; }
        public string TargetPositionName { get; set; }
        public string TargetPositionExecutorNowAgentName { get; set; }
        public string TargetPositionExecutorAgentPhoneNumber { get; set; }

    }
}
