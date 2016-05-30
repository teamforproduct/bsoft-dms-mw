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
        /// ИД
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ИД агента
        /// </summary>
        public int AgentId { get; set; }
        /// <summary>
        /// ссылка на тип адреса
        /// </summary>
        public List<int> AddressTypeId { get; set; }
        /// <summary>
        /// индекс
        /// </summary>
        public string PostCode { get; set; }
        public string PostCodeExact { get; set; }
        /// <summary>
        /// адрес
        /// </summary>
        public string Address { get; set; }
        public string AddressExact { get; set; }

    }
}
