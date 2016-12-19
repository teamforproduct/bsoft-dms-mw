using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace BL.Model.DictionaryCore.IncomingModel
{
    /// <summary>
    /// адреса контрагентов
    /// </summary>
    public class AddAgentAddress
    {
        /// <summary>
        /// Агент (сотрудник, юр.лицо, банк, физ.лицо)
        /// </summary>
        [Required]
        public int AgentId { get; set; }

        /// <summary>
        /// ссылка на тип адреса
        /// </summary>
        [Required]
        public int AddressTypeId { get; set; }

        /// <summary>
        /// индекс
        /// </summary>
        public string PostCode { get; set; }

        /// <summary>
        /// адрес
        /// </summary>
        [Required]
        public string Address { get; set; }

        /// <summary>
        /// комментарии
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// признак активности
        /// </summary>
        [Required]
        public bool IsActive { get; set; }
    }
}
