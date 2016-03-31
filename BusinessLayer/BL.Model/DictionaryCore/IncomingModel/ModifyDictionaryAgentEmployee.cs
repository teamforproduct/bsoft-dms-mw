using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DictionaryCore.IncomingModel
{
    /// <summary>
    /// контрагент - сотрудник
    /// </summary>
    public class ModifyDictionaryAgentEmployee : ModifyDictionaryAgentPerson
    {
        /// <summary>
        /// табельный номер
        /// </summary>
        public string PersonnelNumber { get; set; }
    }
}
