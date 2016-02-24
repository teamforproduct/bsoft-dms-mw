using System;
using BL.Model.Common;

namespace BL.Model.DocumentCore.InternalModel
{
    public class InternalLinkedDocument : LastChangeInfo
    {
        public int? DocumentId { get; set; }
        public int? ParentDocumentId { get; set; }
        public int? DocumentLinkId { get; set; }
        public int? ParentDocumentLinkId { get; set; }
        public int? ExecutorPositionId { get; set; }
        public int LinkTypeId { get; set; }

    }
}