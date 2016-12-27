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
        /// ИД адреса
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Агент (сотрудник, юр.лицо, банк, физ.лицо)
        /// </summary>
        public int? AgentId { get; set; }

        /// <summary>
        /// Тип адреса
        /// </summary>
        public FrontAddressType AddressType { get; set; }
        
        /// <summary>
        /// Индекс
        /// </summary>
        public string PostCode { get; set; }

        /// <summary>
        /// Адрес
        /// </summary>
        public string Address { get; set; }
        
        /// <summary>
        /// Описание
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// признак активности
        /// </summary>
        public bool? IsActive { get; set; }
    }
}
