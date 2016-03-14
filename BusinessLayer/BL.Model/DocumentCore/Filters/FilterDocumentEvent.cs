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

        public int? AgentId { get; set; }

        public int? PositionId { get; set; }

        public EnumEventTypes? EventType { get; set; }

        public EnumImportanceEventTypes? Importance { get; set; }

        public string Description { get; set; }
    }
}
