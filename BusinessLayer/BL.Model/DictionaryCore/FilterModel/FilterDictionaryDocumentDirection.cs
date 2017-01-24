using System.Collections.Generic;

namespace BL.Model.DictionaryCore.FilterModel
{
    public class FilterDictionaryDocumentDirection : DictionaryBaseFilterParameters
    {

        /// <summary>
        /// Сужение по коду (вхождение)
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Сужение по коду (равенство)
        /// </summary>
        public string CodeExact { get; set; }

    }
}
