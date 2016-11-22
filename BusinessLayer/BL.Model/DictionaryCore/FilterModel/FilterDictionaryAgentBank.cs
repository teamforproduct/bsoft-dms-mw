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
    public class FilterDictionaryAgentBank : DictionaryBaseFilterParameters
    {

        /// <summary>
        /// Строка, для полнотекстового поиска
        /// </summary>
        public string FullTextSearchString { get; set; }

        /// <summary>
        /// МФО
        /// </summary>
        public string MFOCode { get; set; }
        public string MFOCodeExact { get; set; }
        public string NameExact { get; set; }
        /// <summary>
        /// Первая буква наименования
        /// </summary>
        public char FirstChar { get; set; }
    }
}
