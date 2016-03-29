using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DocumentCore.Filters
{
    /// <summary>
    /// Фильтр задач по документу
    /// </summary>
    public class FilterDocumentPaperList
    {
        /// <summary>
        /// Массив ИД документов
        /// </summary>
        public List<int> DocumentId { get; set; }
    }
}
