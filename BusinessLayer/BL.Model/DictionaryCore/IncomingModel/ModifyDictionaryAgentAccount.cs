using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace BL.Model.DictionaryCore.IncomingModel
{   /// <summary>
    /// Расчетный счет контрагента
    /// </summary>
    public class ModifyDictionaryAgentAccount
    {
        /// <summary>
        /// ИД
        /// </summary>
        [IgnoreDataMember]
        public int Id { get; set; }
        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Ссылка на банк
        /// </summary>
        public int AgentBankId { get; set; }
        /// <summary>
        /// Ссылка на контрагента
        /// </summary>
        public int AgentId { get; set; }
        /// <summary>
        /// номер счета
        /// </summary>
        public string AccountNumber { get; set; }
        /// <summary>
        /// является основным
        /// </summary>
        public bool IsMain { get; set; }
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
