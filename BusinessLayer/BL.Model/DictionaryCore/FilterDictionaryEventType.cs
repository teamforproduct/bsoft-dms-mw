using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DictionaryCore
{
    /// <summary>
    /// Фильтр словаря типов событий
    /// </summary>
    public class FilterDictionaryEventType
    {
        /// <summary>
        /// Массив ИД типов событий
        /// </summary>
        public List<int> Id { get; set; }
        /// <summary>
        /// Массив ИД типов важности событий
        /// </summary>
        public List<int> ImportanceEventTypeId { get; set; }
        /// <summary>
        /// Массив ИД документов
        /// </summary>
        public List<int> DocumentId { get; set; }       
    }
}
