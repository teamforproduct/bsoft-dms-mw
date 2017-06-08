using BL.Model.Enums;
using System;
using System.Linq;
using System.Xml.Serialization;

namespace BL.Model.SystemCore
{
    public class UIPaging
    {
        /// <summary>
        /// Пагинация. Номер страницы
        /// По умолчанию 1
        /// </summary>
        public int CurrentPage { get; set; } = 1;        
        /// <summary>
        /// Пагинация. Количество записей в странице
        /// По умолчанию 25
        /// </summary>
        public int PageSize { get; set; } = 25;
        /// <summary>
        /// Пагинация. ИД записи, который нужно найти.
        /// Если запись найдена, то будет возвращены записи на странице, на которой она найдена, номер страницы выставится в CurrentPage
        /// Если запись не найдена, то SearchId будет сброшен и будет отработан стандартная пагинация по CurrentPage и PageSize
        /// </summary>
        public int? SearchId { get; set; }
        /// <summary>
        /// Количество записей
        /// </summary>
        [XmlIgnore]
        public int? TotalItemsCount { get; set; }
        /// <summary>
        /// Признак того, что фуллтекст вернул не все записи из-за ограничений и счетчики считаться не будут
        /// </summary>
        [XmlIgnore]
        public bool? IsNotAll { get; set; } = false;
        /// <summary>
        /// Пагинация. Вернуть все данные не применяя пейджинга
        /// По умолчанию false
        /// </summary>
        public bool IsAll { get; set; } = false;
        /// <summary>
        /// Пагинация. Вернет только количество записей если = true
        /// Вернет количество записей и данные если = null
        /// Вернет только данные если = false
        /// По умолчанию null
        /// </summary>
        public bool? IsOnlyCounter { get; set; }
        /// <summary>
        /// Пагинация. Вернет дополнительные счетчики если = true
        /// Вернет дополнительные счетчики и данные если = null
        /// Вернет только данные если = false
        /// По умолчанию null
        /// </summary>
        public bool? IsCalculateAddCounter { get; set; }
        /// <summary>
        /// Второстепенные счетчики
        /// </summary>
        [XmlIgnore]
        public UICounters Counters { get; set; }

        /// <summary>
        /// Пагинация. Тип сортировки
        /// </summary>
        public EnumSort Sort { get; set; }

    }
}