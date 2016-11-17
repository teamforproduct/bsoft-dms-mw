using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DictionaryCore.FilterModel
{
    /// <summary>
    /// фильтр типов контактов
    /// </summary>
    public class FilterDictionaryContactType : DictionaryBaseFilterParameters
    {
        /// <summary>
        /// Строка, для полнотекстового поиска
        /// </summary>
        public string FullTextSearchString { get; set; }

        public string Code {get; set;}

        public string CodeExact { get; set; }

        public string NameExact { get; set; }
         
    }
}
