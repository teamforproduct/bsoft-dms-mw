using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DictionaryCore.FilterModel
{

    /// <summary>
    /// Фильр для контрагентов - банков
    /// </summary>
    public class FilterDictionaryAgentBank
    {

        /// <summary>
        /// Массив ИД 
        /// </summary>
        public List<int> AgentId { get; set; }
        /// <summary>
        /// МФО
        /// </summary>
        public string MFOCode { get; set; }
        /// <summary>
        /// Фрагмент наименования
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// признак активности
        /// </summary>
        public bool? IsActive { get; set; }
    }
}
