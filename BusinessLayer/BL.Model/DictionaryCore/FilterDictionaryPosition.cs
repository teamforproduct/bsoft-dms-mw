using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DictionaryCore
{
    /// <summary>
    /// Фильтр словаря должностей
    /// </summary>
    public class FilterDictionaryPosition
    {
        /// <summary>
        /// Массив ИД должностей
        /// </summary>
        public List<int> Id { get; set; }
        /// <summary>
        /// Массив ИД документов для поиска корреспондентов в событиях
        /// </summary>
        public List<int> DocumentId { get; set; }
    }
}
