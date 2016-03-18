using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DictionaryCore.FrontModel
{
    /// <summary>
    /// адреса контрагентов
    /// </summary>
    public class FrontDictionaryAgentAddress
    {
        /// <summary>
        /// ИД
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ссылка на контрагента
        /// </summary>
        public int AgentId { get; set; }
        /// <summary>
        /// тип адреса
        /// </summary>
        public FrontDictionaryAddressType AddressType { get; set; }
        /// <summary>
        /// Индекс
        /// </summary>
        public string PostCode { get; set; }
        /// <summary>
        /// адрес
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// комментарии
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// признак активности
        /// </summary>
        public bool IsActive { get; set; }
    }
}
