using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace BL.Model.DictionaryCore.IncomingModel
{
    /// <summary>
    /// адреса контрагентов
    /// </summary>
    public class ModifyDictionaryAgentAddress
    {
        /// <summary>
        /// ИД
        /// </summary>
        [IgnoreDataMember]
        public int Id { get; set; }
        /// <summary>
        /// ссылка на контрагента
        /// </summary>
        public int AgentId { get; set; }
        /// <summary>
        /// ссылка на тип адреса
        /// </summary>
        public int AddressTypeId { get; set; }
        /// <summary>
        /// индекс
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
