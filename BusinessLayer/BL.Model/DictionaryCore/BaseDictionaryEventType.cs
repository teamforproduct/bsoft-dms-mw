using BL.Model.Enums;
using System;

namespace BL.Model.DictionaryCore
{
    public class BaseDictionaryEventType
    {
        public EnumEventTypes EventType { get; set; }
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int ImportanceEventTypeId { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        public string ImportanceEventTypeName { get; set; }
    }
}