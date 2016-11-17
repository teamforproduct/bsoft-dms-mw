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
        /// Агент (сотрудник, юр.лицо, банк, физ.лицо, компания)
        /// </summary>
        public List<int> AgentIDs { get; set; }

        /// <summary>
        /// не содержит агентов 
        /// </summary>
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
        public string ContactExact { get; set; }
    }
}
