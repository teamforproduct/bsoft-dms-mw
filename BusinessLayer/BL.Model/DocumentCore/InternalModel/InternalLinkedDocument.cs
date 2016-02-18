namespace BL.Model.InternalModel
{
    public class InternalLinkedDocument
    {
        public int DocumentId { get; set; }
        public int ParentDocumentId { get; set; }
        public int? DocumentLinkId { get; set; }
        public int? ParentDocumentLinkId { get; set; }
        public int ExecutorPositionId { get; set; }
        public int LinkTypeId { get; set; }
    }
}