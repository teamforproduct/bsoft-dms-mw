using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Model.Common;
using BL.Model.DictionaryCore.IncomingModel;

namespace BL.Model.DictionaryCore.InternalModel
{
    /// <summary>
    /// Расчетные счета
    /// </summary>
    public class InternalDictionaryAgentAccount : LastChangeInfo
    {

        public InternalDictionaryAgentAccount()
        { }

        public InternalDictionaryAgentAccount(ModifyDictionaryAgentAccount model)
        {
            Id = model.Id;
            AgentId = model.AgentId;
            AgentBankId = model.AgentBankId;
            Name = model.Name;
            AccountNumber = model.AccountNumber;
            IsMain = model.IsMain;
            Description = model.Description;
            IsActive = model.IsActive;
        }

        /// <summary>
        /// ИД
        /// </summary>
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
        /// ссылка на контрагента
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
