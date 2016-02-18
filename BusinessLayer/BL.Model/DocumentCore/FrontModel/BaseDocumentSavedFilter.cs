using System;

namespace BL.Model.DocumentCore.FrontModel
{
    public class BaseDocumentSavedFilter
    {
        public int Id { get; set; }
        public Nullable<int> PositionId { get; set; }
        public string Icon { get; set; }
        public object Filter { get; set; }
        public bool IsCommon { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        public string PositionName { get; set; }
    }
}
