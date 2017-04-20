using System;
using BL.Model.Common;
using BL.Model.Enums;

namespace BL.Model.DocumentCore.InternalModel
{
    public class InternalDocumentRestrictedSendList : LastChangeInfo
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int EntityTypeId { get; set; }
        public int DocumentId { get; set; }
        public int? PositionId { get; set; }
        public EnumAccessLevels AccessLevel { get; set; }

    }
}
