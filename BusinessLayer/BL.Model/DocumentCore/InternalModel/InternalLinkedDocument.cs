using System;

namespace BL.Model.DocumentCore.InternalModel
{
    public class InternalLinkedDocument
    {
        public int DocumentId { get; set; }
        public int ParentDocumentId { get; set; }
        public int? DocumentLinkId { get; set; }
        public int? ParentDocumentLinkId { get; set; }
        public int ExecutorPositionId { get; set; }
        public int LinkTypeId { get; set; }
        public DateTime LastChangeDate { get; set; }
    }
}