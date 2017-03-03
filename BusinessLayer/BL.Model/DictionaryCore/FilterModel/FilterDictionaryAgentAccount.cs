using BL.Model.Common;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BL.Model.DictionaryCore.FilterModel
{
    /// <summary>
    /// Фильтр для счетов контрагента
    /// </summary>
    public class FilterDictionaryAgentAccount : BaseFilterNameIsActive
    {
       
        /// <summary>
        /// номер счета
        /// </summary>
        public string AccountNumber { get; set; }

        /// <summary>
        /// номер счета
        /// </summary>
        [IgnoreDataMember]
        public string AccountNumberExact { get; set; }

        /// <summary>
        /// банк
        /// </summary>
        public List<int> AgentIDs { get; set; }
       
    }
}
