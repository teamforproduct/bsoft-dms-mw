using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DictionaryCore.FilterModel
{
    /// <summary>
    /// фильтр типов контактов
    /// </summary>
    public class FilterDictionaryContactType
    {
        /// <summary>
        /// список ИД
        /// </summary>
        public List<int> ContactTypeId { get; set; }
        /// <summary>
        /// фррагмент наименования
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// признак активности
        /// </summary>
        public bool? IsActive { get; set; }
        /// <summary>
        /// игнорировать при поиске
        /// </summary>
        public List<int> NotContainsId { get; set; }
    }
}
