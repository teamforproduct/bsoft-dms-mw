using System;
using BL.Model.Enums;

namespace BL.Model.DocumentCore.FrontModel
{
    public class FrontDocumentAccess
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public int? PositionId { get; set; }
        public int AccessLevelId { get; set; }
        public string AccessLevelName { get; set; }
        public bool IsInWork { get; set; }
        public bool IsFavourite { get; set; }
        public int? CountNewEvents { get; set; }
        public int? CountWaits { get; set; }
        public int? OverDueCountWaits { get; set; }
        public DateTime? MinDueDate { get; set; }
    }
}
