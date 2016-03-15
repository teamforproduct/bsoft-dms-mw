using System;

namespace BL.Model.DocumentCore.FrontModel
{
    /// <summary>
    /// Модель для отображения информации в списке ивентов документа
    /// </summary>
    public class FrontDocumentEventList
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public string RegistrationFullNumber { get; set; }
        public DateTime DocumentDate { get; set; }
        public string EventTypeName { get; set; }
        public DateTime Date { get; set; }
        public string Task { get; set; }
        public string Description { get; set; }
        public DateTime? DueDate { get; set; }
        public string SourceAgentName { get; set; }
        public string SourcePositionExecutorAgentName { get; set; }
        public string TargetAgentName { get; set; }
        public string TargetPositionExecutorAgentName { get; set; }
    }
}