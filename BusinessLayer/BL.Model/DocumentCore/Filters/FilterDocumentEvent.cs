using System.Collections.Generic;
using BL.Model.Enums;

namespace BL.Model.DocumentCore.Filters
{
    /// <summary>
    /// Фильтр событий документов
    /// </summary>
    public class FilterDocumentEvent
    {
        /// <summary>
        /// Массив ИД событий документов
        /// </summary>
        public List<int> EventId { get; set; }
        /// <summary>
        /// Массив ИД документов
        /// </summary>
        public List<int> ListDocumentId { get; set; }

        /// <summary>
        /// when you want to get events from the one document, you can do it faster with that field
        /// </summary>
        public int? DocumentId { get; set; }

        /// <summary>
        /// список агентов, для которых ищуться ивенты
        /// </summary>
        public List<int> AgentId { get; set; }

        /// <summary>
        /// список позиций, для которых ищеться ивенты
        /// </summary>
        public List<int> PositionId { get; set; }

        /// <summary>
        /// тип события
        /// </summary>
        public List<EnumEventTypes> EventType { get; set; }

        /// <summary>
        /// Тип важности события
        /// </summary>
        public List<EnumImportanceEventTypes> Importance { get; set; }

        /// <summary>
        /// Поиск значения в описании ивента (по совпадению) 
        /// </summary>
        public string Description { get; set; }
    }
}
