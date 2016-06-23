using System.Xml.Serialization;

namespace BL.Model.SystemCore
{
    public class UIPaging
    {
        /// <summary>
        /// Номер страницы
        /// По умолчанию 1
        /// </summary>
        public int CurrentPage { get; set; } = 1;
        /// <summary>
        /// Количество записей в странице
        /// По умолчанию 25
        /// </summary>
        public int PageSize { get; set; } = 25;
        [XmlIgnore]
        public int TotalItemsCount { get; set; }
        /// <summary>
        /// Вернуть все данные не применяя пейджинга
        /// По умолчанию false
        /// </summary>
        public bool IsAll { get; set; } = false;
        /// <summary>
        /// Вернет только количество записей если = true
        /// Вернет количество записей и данные если = null
        /// Вернет только данные если = false
        /// По умолчанию null
        /// </summary>
        public bool? IsOnlyCounter { get; set; }
        [XmlIgnore]
        public UICounters Counters { get; set; }
    }
}