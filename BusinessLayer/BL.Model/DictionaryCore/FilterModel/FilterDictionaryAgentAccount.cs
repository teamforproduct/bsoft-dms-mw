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
    public class FilterDictionaryAgentAccount : DictionaryBaseFilterParms
    {
       
        /// <summary>
        /// номер счета
        /// </summary>
        public string AccountNumber { get; set; }
        /// <summary>
        /// банк
        /// </summary>
        public int? AgentBankId { get; set; }
       
    }
}
