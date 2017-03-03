using BL.Model.Common;
using System.Collections.Generic;

namespace BL.Model.DictionaryCore.FilterModel
{
    public class FilterDictionarySendType : BaseFilterNameIsActive
    {
        /// <summary>
        /// Сужение по кодам
        /// </summary>
        public List<string> Codes { get; set; }

    }
}
