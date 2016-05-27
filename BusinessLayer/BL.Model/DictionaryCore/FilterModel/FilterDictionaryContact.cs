using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DictionaryCore.FilterModel
{
    /// <summary>
    /// фильтр контактов
    /// </summary>
    public class FilterDictionaryContact : DictionaryBaseFilterParameters
    {
        /// <summary>
        /// ИД
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ссылка на контрагента
        /// </summary>
        public List<int> AgentId { get; set; }
        /// <summary>
        /// ссылка на тип контакта
        /// </summary>
        public List<int> ContactTypeId { get; set; }
        /// <summary>
        /// значение
        /// </summary>
        public string Contact { get; set; }
        public string ContactExact { get; set; }
    }
}
