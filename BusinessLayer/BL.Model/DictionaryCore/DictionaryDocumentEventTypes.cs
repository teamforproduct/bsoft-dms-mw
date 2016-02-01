using System;

namespace BL.Model.DictionaryCore
{
    public class DictionaryDocumentEventTypes
    {
        public DocumentEventTypes EventType { get; set; }
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int ImpotanceEventTypeId { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        public string ImpotanceEventTypeName { get; set; }
    }
}