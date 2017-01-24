using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DictionaryCore.FrontModel
{
    /// <summary>
    /// Расчетный счет контрагента
    /// </summary>
    public class FrontDictionaryAgentAccount
    {
        /// <summary>
        /// ИД
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Банк, в котором открыт счет
        /// </summary>
        public FrontMainAgentBank Bank { get; set; }
        /// <summary>
        /// ССылка на контрагента
        /// </summary>
        public int? AgentId {get; set;}
        /// <summary>
        /// номер счета
        /// </summary>
        public string AccountNumber { get; set; }
        /// <summary>
        /// является основным
        /// </summary>
        public bool? IsMain { get; set; }
        /// <summary>
        /// комментарии
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// признак активности
        /// </summary>
        public bool? IsActive { get; set; }
    }
}
