using System;
using BL.Model.Enums;

namespace BL.Model.DocumentCore
{
    public class BaseDocumentAccess
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public int PositionId { get; set; }
        public EnumDocumentAccesses AccessLevel { get; set; }
        public string AccessLevelName { get; set; }
        public bool IsInWork { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        public bool IsFavourite { get; set; }
    }
}
