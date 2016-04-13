using System;
using BL.Model.Enums;

namespace BL.Model.DocumentCore.FrontModel
{
    /// <summary>
    /// Describe the display model for the Document events.
    /// </summary>
    public class FrontDocumentEvent : FrontDocumentInfo
    {
        public int Id { get; set; }

        public int? EventType { get; set; }
        public string EventTypeName { get; set; }
        public DateTime? Date { get; set; }
        public string Task { get; set; }
        public bool? IsAvailableWithinTask { get; set; }
        public string Description { get; set; }
        public DateTime? DueDate { get; set; }


        public DateTime? ReadDate { get; set; }
        public string ReadAgentName { get; set; }


        public string SourceAgentName { get; set; }
        public string SourcePositionExecutorAgentName { get; set; }
        public string TargetAgentName { get; set; }
        public string TargetPositionExecutorAgentName { get; set; }
        public string SourcePositionExecutorNowAgentName { get; set; }
        public string SourcePositionName { get; set; }
        public string SourcePositionExecutorAgentPhoneNumber { get; set; }

        public string TargetPositionName { get; set; }
        public string TargetPositionExecutorNowAgentName { get; set; }
        public string TargetPositionExecutorAgentPhoneNumber { get; set; }

        public int? PaperId { get; set; }
        public string PaperName { get; set; }
        public bool? PaperIsMain { get; set; }
        public bool? PaperIsOriginal { get; set; }
        public bool? PaperIsCopy { get; set; }
        public int? PaperOrderNumber { get; set; }

        public DateTime? PaperPlanDate { get; set; }
        public DateTime? PaperSendDate { get; set; }
        public DateTime? PaperRecieveDate { get; set; }

        public string PaperPlanAgentName { get; set; }
        public string PaperSendAgentName { get; set; }
        public string PaperRecieveAgentName { get; set; }




    }
}
