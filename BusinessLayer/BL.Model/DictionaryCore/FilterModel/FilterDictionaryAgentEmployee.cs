using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DictionaryCore.FilterModel
{
    /// <summary>
    /// фильтр сотрудников
    /// </summary>
    public class FilterDictionaryAgentEmployee : FilterDictionaryAgentPerson
    {
        /// <summary>
        /// табельный номер
        /// </summary>
        public string PersonnelNumber { get; set; }
       

    }
}
