using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Model.Common;
using BL.Model.Enums;

namespace BL.Model.DocumentCore.InternalModel
{
    public class InternalDocumentPaperEvent : LastChangeInfo
    {
        public int Id { get; set; }
        public int PaperId { get; set; }
        public int SendTypeId { get; set; }
        public EnumEventTypes EventType { get; set; }
        public Nullable<int> EventId { get; set; }
        public string Description { get; set; }
        public Nullable<int> SourcePositionId { get; set; }
        public Nullable<int> SourcePositionExecutorAgentId { get; set; }
        public Nullable<int> SourceAgentId { get; set; }
        public Nullable<int> TargetPositionId { get; set; }
        public Nullable<int> TargetPositionExecutorAgentId { get; set; }
        public Nullable<int> TargetAgentId { get; set; }
        public Nullable<int> PaperListId { get; set; }
        public int PlanAgentId { get; set; }
        public DateTime PlanDate { get; set; }
        public Nullable<int> SendAgentId { get; set; }
        public Nullable<DateTime> SendDate { get; set; }
        public Nullable<int> RecieveAgentId { get; set; }
        public Nullable<DateTime> RecieveDate { get; set; }
    }
}
