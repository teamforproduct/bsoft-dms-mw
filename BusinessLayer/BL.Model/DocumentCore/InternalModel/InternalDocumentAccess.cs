using System;
using BL.Model.Common;
using BL.Model.Enums;

namespace BL.Model.DocumentCore.InternalModel
{
    public class InternalDocumentAccess : LastChangeInfo
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int EntityTypeId { get; set; }
        public int DocumentId { get; set; }
        public int? PositionId { get; set; }
        public EnumDocumentAccesses AccessLevel { get; set; }
        public bool IsInWork { get; set; }
        public bool IsFavourite { get; set; }

      //  public InternalDocumentEvent DocumentEvent { get; set; }

    }
}
