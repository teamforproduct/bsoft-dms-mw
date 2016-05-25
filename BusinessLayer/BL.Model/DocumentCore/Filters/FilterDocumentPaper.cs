using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DocumentCore.Filters
{
    /// <summary>
    /// Фильтр бумажных носителей по документу
    /// </summary>
    public class FilterDocumentPaper
    {
        /// <summary>
        /// Массив ИД бумажных носителей
        /// </summary>
        public List<int> Id { get; set; }
        /// <summary>
        /// Массив ИД документов
        /// </summary>
        public List<int> DocumentId { get; set; }
        /// <summary>
        /// Реестр БН
        /// </summary>
        public int? PaperListId { get; set; }
    }
}
