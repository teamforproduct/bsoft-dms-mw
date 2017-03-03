using BL.Model.Common;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BL.Model.DictionaryCore.FilterModel
{
    /// <summary>
    /// фильтр для адресов агентов
    /// </summary>
    public class FilterDictionaryAgentAddress : BaseFilterNameIsActive
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
        [IgnoreDataMember]
        public string PostCodeExact { get; set; }

        /// <summary>
        /// по адресу (вхождение)
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// по адресу (совпадение)
        /// </summary>
        [IgnoreDataMember]
        public string AddressExact { get; set; }

    }
}
