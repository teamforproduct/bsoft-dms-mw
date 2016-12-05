using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DictionaryCore.FilterModel
{
    /// <summary>
    /// фильтр для адресов агентов
    /// </summary>
    public class FilterDictionaryAgentAddress : DictionaryBaseFilterParameters
    {

        /// <summary>
        /// Агент (сотрудник, юр.лицо, банк, физ.лицо)
        /// </summary>
        public List<int> AgentIDs { get; set; }

        /// <summary>
        /// По типу адреса
        /// </summary>
        public List<int> AddressTypeIDs { get; set; }
        
        /// <summary>
        /// по индексу (вхождение)
        /// </summary>
        public string PostCode { get; set; }

        /// <summary>
        /// по индексу (совпадение)
        /// </summary>
        public string PostCodeExact { get; set; }

        /// <summary>
        /// по адресу (вхождение)
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// по адресу (совпадение)
        /// </summary>
        public string AddressExact { get; set; }

    }
}
