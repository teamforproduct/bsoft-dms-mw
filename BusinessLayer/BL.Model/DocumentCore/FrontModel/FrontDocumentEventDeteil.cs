using System;

namespace BL.Model.DocumentCore.FrontModel
{
    /// <summary>
    /// Модель для отображения деталей по ивенту документа
    /// </summary>
    public class FrontDocumentEventDeteil
    {
        public int Id { get; set; }
        public DateTime? ReadDate { get; set; }
        public string ReadAgentName { get; set; }
        public string SourceAgentName { get; set; }
        public string SourcePositionExecutorAgentName { get; set; }
        public string SourcePositionName { get; set; }
        public string SourcePositionPhone { get; set; }
        public string TargetPositionName { get; set; }
        public string TargetPositionExecutorAgentName { get; set; }
        public string TargetPositionPhone { get; set; }
        public string DocumentTypeName { get; set; }
        public string DocumentDescription { get; set; }
    }
}