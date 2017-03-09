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
        /// Количество записей
        /// </summary>
        [XmlIgnore]
        public int? TotalItemsCount { get; set; }

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