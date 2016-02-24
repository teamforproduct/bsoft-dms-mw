using System;
using BL.Model.Common;
using BL.Model.Enums;

namespace BL.Model.DocumentCore.InternalModel
{
    public class InternalDocumentAccesses : LastChangeInfo
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public int PositionId { get; set; }
        public EnumDocumentAccesses AccessLevel { get; set; }
        public bool IsInWork { get; set; }
        public bool IsFavourite { get; set; }

        public InternalDocumentEvents DocumentEvent { get; set; }

    }
}
