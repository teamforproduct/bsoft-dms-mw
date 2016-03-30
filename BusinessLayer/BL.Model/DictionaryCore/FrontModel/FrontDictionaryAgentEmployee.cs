using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DictionaryCore.FrontModel
{
    /// <summary>
    /// Контрагент - сотрудник
    /// </summary>
    public class FrontDictionaryAgentEmployee : FrontDictionaryAgentPerson
    {
        /// <summary>
        /// табельный номер
        /// </summary>
        public string PersonnelNumber { get; set; }
    }
}
