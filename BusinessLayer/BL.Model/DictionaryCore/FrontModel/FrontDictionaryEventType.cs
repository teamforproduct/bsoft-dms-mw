using BL.Model.Enums;
using System;

namespace BL.Model.DictionaryCore.FrontModel
{
    /// <summary>
    /// Типы событий
    /// </summary>
    public class FrontDictionaryEventType
    {
        /// <summary>
        /// Тип события
        /// </summary>
        public EnumEventTypes EventType { get; set; }
        /// <summary>
        /// ИД
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Код
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Важность события 
        /// </summary>
        public int ImportanceEventTypeId { get; set; }
        /// <summary>
        /// Пользователь
        /// </summary>
        public int LastChangeUserId { get; set; }
        /// <summary>
        /// Дата изменения
        /// </summary>
        public DateTime LastChangeDate { get; set; }
        /// <summary>
        /// Название типа важности
        /// </summary>
        public string ImportanceEventTypeName { get; set; }
    }
}