using BL.Model.Common;
using BL.Model.Enums;

namespace BL.Model.DictionaryCore.FrontModel
{
    /// <summary>
    /// Типы событий
    /// </summary>
    public class FrontDictionaryEventType : ListItem
    {
        /// <summary>
        /// Тип события
        /// </summary>
        public EnumEventTypes EventType { get; set; }
        /// <summary>
        /// Код
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// Важность события 
        /// </summary>
        public int ImportanceEventTypeId { get; set; }
        /// <summary>
        /// Название типа важности
        /// </summary>
        public string ImportanceEventTypeName { get; set; }
    }
}