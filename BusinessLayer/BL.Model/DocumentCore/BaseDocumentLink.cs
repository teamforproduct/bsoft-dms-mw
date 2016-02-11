using System;

namespace BL.Model.DocumentCore
{
    public class ВaseDocumentLink
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public int ParentDocumentId { get; set; }
        public int LinkTypeId { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        public string LinkTypeName { get; set; }
    }
}
