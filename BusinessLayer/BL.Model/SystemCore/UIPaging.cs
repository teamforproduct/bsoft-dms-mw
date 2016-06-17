using System.Xml.Serialization;

namespace BL.Model.SystemCore
{
    public class UIPaging
    {
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 25;
        [XmlIgnore]
        public int TotalItemsCount { get; set; }
        public bool IsAll { get; set; } = false;
        /// <summary>
        /// Вернет только количество записей если = true
        /// </summary>
        public bool? IsOnlyCounter { get; set; }
        [XmlIgnore]
        public UICounters Counters { get; set; }
    }
}