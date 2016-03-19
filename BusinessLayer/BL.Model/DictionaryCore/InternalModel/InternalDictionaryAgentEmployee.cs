using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DictionaryCore.InternalModel
{
    /// <summary>
    /// контрагент - сотрудник
    /// </summary>
    public class InternalDictionaryAgentEmployee : InternalDictionaryAgentPerson
    {
        /// <summary>
        /// табельный номер
        /// </summary>
        public string PersonnelNumber { get; set; }
    }
}
