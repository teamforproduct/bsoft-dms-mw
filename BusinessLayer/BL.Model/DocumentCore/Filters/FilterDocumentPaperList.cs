using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DocumentCore.Filters
{
    /// <summary>
    /// Фильтр реестров бумажных носителей
    /// </summary>
    public class FilterDocumentPaperList
    {
        /// <summary>
        /// Массив ИД реестров БН
        /// </summary>
        public List<int> PaperListId { get; set; }
    }
}
