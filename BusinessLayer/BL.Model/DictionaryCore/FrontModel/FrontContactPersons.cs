using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Model.Enums;
using System.Runtime.Serialization;
using BL.Model.DictionaryCore.IncomingModel;

namespace BL.Model.DictionaryCore.FrontModel
{

    /// <summary>
    /// Контактные лица
    /// </summary>
    public class FrontContactPersons : ModifyAgentContactPerson
    {

        /// <summary>
        /// Список контактов контрагента
        /// </summary>
        public IEnumerable<FrontDictionaryAgentContact> Contacts { get; set; }

    }
}
