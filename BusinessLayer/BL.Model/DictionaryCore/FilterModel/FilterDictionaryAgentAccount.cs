using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DictionaryCore.FilterModel
{
    /// <summary>
    /// Фильтр для счетов контрагента
    /// </summary>
    public class FilterDictionaryAgentAccount : DictionaryBaseFilterParameters
    {
       
        /// <summary>
        /// номер счета
        /// </summary>
        public string AccountNumber { get; set; }

        /// <summary>
        /// номер счета
        /// </summary>
        public string AccountNumberExact { get; set; }

        /// <summary>
        /// банк
        /// </summary>
        public List<int> AgentIDs { get; set; }
       
    }
}
