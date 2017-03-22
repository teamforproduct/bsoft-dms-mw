using BL.Model.Common;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BL.Model.DictionaryCore.FilterModel
{
    /// <summary>
    /// фильтр контактов
    /// </summary>
    public class FilterDictionaryContact : BaseFilterNameIsActive
    {

        /// <summary>
        /// Агент (сотрудник, юр.лицо, банк, физ.лицо, компания)
        /// </summary>
        public List<int> AgentIDs { get; set; }

        /// <summary>
        /// не содержит агентов 
        /// </summary>
        [IgnoreDataMember]
        public List<int> NotContainsAgentIDs { get; set; }
        
        /// <summary>
        /// по типам контактов
        /// </summary>
        public List<int> ContactTypeIDs { get; set; }

        /// <summary>
        /// Сужение по подтвержденным контактам
        /// </summary>
        public bool? IsConfirmed { get; set; }

        /// <summary>
        /// контакт (по вхождению)
        /// </summary>
        public string Contact { get; set; }

        /// <summary>
        /// контакт (по равенству)
        /// </summary>
        [IgnoreDataMember]
        public string ContactExact { get; set; }
    }
}
