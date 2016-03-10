using System;
using BL.Model.Enums;

namespace BL.Model.SystemCore
{
    /// <summary>
    /// Describe data structure for making Emails with notifications for users
    /// </summary>
    public class InternalDataForMail
    {
        public int EventId { get; set; }
        public int DocumentId { get; set; }
        public string DocumentName { get; set; }
        public int SourceAgentId { get; set; }
        public string SourceAgentName { get; set; }
        public int SourcePositiontId { get; set; }
        public string SourcePositionName { get; set; }
        public int DestinationAgentId { get; set; }
        public string DestinationAgentName { get; set; }
        public int DestinationPositionId { get; set; }
        public string DestinationPositionName { get; set; }
        public string DestinationAgentEmail { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public EnumEventTypes EventType { get; set; }
        public bool WasUpdated { get; set; }
    }
}