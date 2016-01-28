using System;

namespace BL.Model.DocumentCore
{
    public class ModifyDocument
    {
        public int Id { get; set; }
        public int TemplateDocumentId { get; set; }
        public int? DocumentSubjectId { get; set; }
        public string Description { get; set; }
        public int ExecutorPositionId { get; set; }
        public int ExecutorAgentId { get; set; }
        public int? SenderAgentId { get; set; }
        public string SenderPerson { get; set; }
        public string SenderNumber { get; set; }
        public DateTime? SenderDate { get; set; }
        public string Addressee { get; set; }
        public int AccessLevelId { get; set; }
    }
}
