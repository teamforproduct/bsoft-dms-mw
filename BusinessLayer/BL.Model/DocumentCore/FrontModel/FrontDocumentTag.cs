using System;

namespace BL.Model.DocumentCore.FrontModel
{
    public class FrontDocumentTag
    {
        public int TagId { get; set; }
        public int DocumentId { get; set; }
        public int? PositionId { get; set; }
        public string PositionName { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public bool IsSystem { get; set; }
        public int? DocCount { get; set; }
    }
}
