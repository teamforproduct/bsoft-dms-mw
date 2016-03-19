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
    public class FilterDictionaryContact
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
        public string Value { get; set; }
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
