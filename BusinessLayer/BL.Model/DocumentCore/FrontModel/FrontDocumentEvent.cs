using System;
using BL.Model.Enums;

namespace BL.Model.DocumentCore.FrontModel
{
    /// <summary>
    /// Describe the display model for the Document events.
    /// </summary>
    public class FrontDocumentEvent
    {
        public int Id { get; set; }
        public int? DocumentId { get; set; }
        public string RegistrationFullNumber { get; set; }
        public DateTime? DocumentDate { get; set; }
        public int? EventType { get; set; }
        public string EventTypeName { get; set; }
        public DateTime? Date { get; set; }
        public string Task { get; set; }
        public string Description { get; set; }
        public DateTime? DueDate { get; set; }
        public string SourceAgentName { get; set; }
        public string SourcePositionExecutorAgentName { get; set; }
        public string TargetAgentName { get; set; }
        public string TargetPositionExecutorAgentName { get; set; }

        public DateTime? ReadDate { get; set; }
        public string ReadAgentName { get; set; }

        public string SourcePositionExecutorNowAgentName { get; set; }
        public string SourcePositionName { get; set; }
        public string SourcePositionExecutorAgentPhoneNumber { get; set; }

        public string TargetPositionName { get; set; }
        public string TargetPositionExecutorNowAgentName { get; set; }
        public string TargetPositionExecutorAgentPhoneNumber { get; set; }

        public string DocumentDirectionName { get; set; }
        public string DocumentTypeName { get; set; }
        public string DocumentDescription { get; set; }
    }
}
