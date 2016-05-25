using System.Collections.Generic;
using BL.Model.Users;

namespace BL.Model.DocumentCore.Actions
{
    /// <summary>
    /// Модель перечня бумажных носителей
    /// </summary>
    public class PaperList
    {
        /// <summary>
        /// Массив ИД БН
        /// </summary>
        public List<int> PaperId { get; set; }
        /// <summary>
        /// Реестр БН
        /// </summary>
        public int? PaperListId { get; set; }
    }
}
