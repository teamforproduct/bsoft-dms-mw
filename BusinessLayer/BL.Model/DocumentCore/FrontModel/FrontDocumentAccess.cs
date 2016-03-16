using System;
using BL.Model.Enums;

namespace BL.Model.DocumentCore.FrontModel
{
    public class FrontDocumentAccess
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public int PositionId { get; set; }
        public EnumDocumentAccesses AccessLevel { get; set; }
        public string AccessLevelName { get; set; }
        public bool IsInWork { get; set; }
        public bool IsFavourite { get; set; }
    }
}
