using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Model.Enums;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace BL.Model.DictionaryCore.IncomingModel
{
    /// <summary>
    /// Контакт агента
    /// </summary>
    public class AddDictionaryContactPersonContact : AddDictionaryContact
    {
        /// <summary>
        /// Агент (сотрудник, юр.лицо, банк, физ.лицо, компания)
        /// </summary>
        [IgnoreDataMember]
        public new int AgentId { get; set; }
        
    }
}
